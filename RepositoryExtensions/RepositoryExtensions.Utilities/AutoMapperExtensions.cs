using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.Impl;
using AutoMapper.Internal;

namespace RepositoryExtensions.Utilities
{
    public static class AutoMapperExtensions
    {
        private static readonly IDictionaryFactory DictionaryFactory = PlatformAdapter.Resolve<IDictionaryFactory>();

        private static readonly AutoMapper.Internal.IDictionary<TypePair, LambdaExpression> _expressionCache
            = DictionaryFactory.CreateDictionary<TypePair, LambdaExpression>();

        /// <summary>
        /// Create an expression tree representing a mapping from the <typeparamref name="TSource"/> type to <typeparamref name="TDestination"/> type
        /// Includes flattening and expressions inside MapFrom member configuration
        /// </summary>
        /// <typeparam name="TSource">Source Type</typeparam>
        /// <typeparam name="TDestination">Destination Type</typeparam>
        /// <param name="mappingEngine">Mapping engine instance</param>
        /// <returns>Expression tree mapping source to destination type</returns>
        public static Expression<Func<TSource, TDestination>> CreateMapExpression<TSource, TDestination>(this IMappingEngine mappingEngine)
        {
            return (Expression<Func<TSource, TDestination>>)
                _expressionCache.GetOrAdd(new TypePair(typeof(TSource), typeof(TDestination)), tp =>
                {
                    if (tp.DestinationType.IsInterface) return CreateMapExpression<TDestination>(mappingEngine, tp.SourceType);
                    return CreateMapExpression(mappingEngine, tp.SourceType, tp.DestinationType);
                });
        }


        /// <summary>
        /// Extention method to project from a queryable using the static <see cref="Mapper.Engine"/> property
        /// Due to generic parameter inference, you need to call Project().To to execute the map
        /// </summary>
        /// <remarks>Projections are only calculated once and cached</remarks>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <param name="source">Queryable source</param>
        /// <returns>Expression to project into</returns>
        public static IProjectionExpression Project<TSource>(
            this IQueryable<TSource> source)
        {
            return source.Project(Mapper.Engine);
        }

        /// <summary>
        /// Extention method to project from a queryable using the provided mapping engine
        /// Due to generic parameter inference, you need to call Project().To to execute the map
        /// </summary>
        /// <remarks>Projections are only calculated once and cached</remarks>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <param name="source">Queryable source</param>
        /// <param name="mappingEngine">Mapping engine instance</param>
        /// <returns>Expression to project into</returns>
        public static IProjectionExpression Project<TSource>(
            this IQueryable<TSource> source, IMappingEngine mappingEngine)
        {
            return new ProjectionExpression<TSource>(source, mappingEngine);
        }

        private static LambdaExpression CreateMapExpression<T>(IMappingEngine mappingEngine, Type typeIn)
        {
            // this is the input parameter of this expression with name <variableName>
            ParameterExpression instanceParameter = Expression.Parameter(typeIn, "dto");

            var total = CreateMapExpression<T>(mappingEngine, typeIn, instanceParameter);

            return Expression.Lambda(total, instanceParameter);
        }

        private static LambdaExpression CreateMapExpression(IMappingEngine mappingEngine, Type typeIn, Type typeOut)
        {
            // this is the input parameter of this expression with name <variableName>
            ParameterExpression instanceParameter = Expression.Parameter(typeIn, "dto");

            var total = CreateMapExpression(mappingEngine, typeIn, typeOut, instanceParameter);

            return Expression.Lambda(total, instanceParameter);
        }

        private static Expression CreateMapExpression<T>(IMappingEngine mappingEngine, Type typeIn, Expression instanceParameter)
        {
            var typeOut = typeof(T);
            var typeMap = mappingEngine.ConfigurationProvider.FindTypeMapFor(typeIn, typeOut);

            if (typeMap == null)
            {
                const string MessageFormat = "Missing map from {0} to {1}. Create using Mapper.CreateMap<{0}, {1}>.";

                var message = string.Format(MessageFormat, typeIn.Name, typeOut.Name);

                throw new InvalidOperationException(message);
            }

            var bindings = CreateMemberBindings(mappingEngine, typeIn, typeMap, instanceParameter);

            Expression total = null;
            if (typeOut.IsInterface && typeMap.DestinationType != null)
            {
                total = Expression.MemberInit(
                    Expression.New(new InterfaceConstructorInfo<T>(
                        new InterfaceType(typeOut, typeMap.DestinationType))),
                    bindings.ToArray()
                    );
            }
            else
            {
                total = Expression.MemberInit(
                    Expression.New(typeOut),
                    bindings.ToArray()
                    );
            }

            return total;
        }

        private static Expression CreateMapExpression(IMappingEngine mappingEngine, Type typeIn, Type typeOut, Expression instanceParameter)
        {
            var typeMap = mappingEngine.ConfigurationProvider.FindTypeMapFor(typeIn, typeOut);

            if (typeMap == null)
            {
                const string MessageFormat = "Missing map from {0} to {1}. Create using Mapper.CreateMap<{0}, {1}>.";

                var message = string.Format(MessageFormat, typeIn.Name, typeOut.Name);

                throw new InvalidOperationException(message);
            }

            var bindings = CreateMemberBindings(mappingEngine, typeIn, typeMap, instanceParameter);

            Expression total = Expression.MemberInit(
                Expression.New(typeOut),
                bindings.ToArray()
                );

            return total;
        }

        private static List<MemberBinding> CreateMemberBindings(IMappingEngine mappingEngine, Type typeIn, TypeMap typeMap,
                                                 Expression instanceParameter)
        {
            var bindings = new List<MemberBinding>();
            foreach (var propertyMap in typeMap.GetPropertyMaps().Where(pm => pm.CanResolveValue()))
            {
                var result = propertyMap.ResolveExpression(typeIn, instanceParameter);

                var destinationMember = propertyMap.DestinationProperty.MemberInfo;

                MemberAssignment bindExpression;

                if (propertyMap.DestinationPropertyType.IsAssignableFrom(result.Type))
                {
                    bindExpression = Expression.Bind(destinationMember, result.ResolutionExpression);
                }
                else if (propertyMap.DestinationPropertyType.GetInterfaces().Any(t => t.Name == "IEnumerable") &&
                    propertyMap.DestinationPropertyType != typeof(string))
                {
                    Type destinationListType = GetDestinationListTypeFor(propertyMap);
                    Type sourceListType = null;
                    // is list

                    sourceListType = result.Type.GetGenericArguments().First();

                    //var newVariableName = "t" + (i++);
                    var transformedExpression = CreateMapExpression(mappingEngine, sourceListType, destinationListType);

                    MethodCallExpression selectExpression = Expression.Call(
                        typeof(Enumerable),
                        "Select",
                        new[] { sourceListType, destinationListType },
                        result.ResolutionExpression,
                        transformedExpression);

                    if (typeof(IList<>).MakeGenericType(destinationListType).IsAssignableFrom(propertyMap.DestinationPropertyType))
                    {
                        var toListCallExpression = GetToListCallExpression(propertyMap, destinationListType, selectExpression);
                        bindExpression = Expression.Bind(destinationMember, toListCallExpression);
                    }
                    else
                    {
                        // destination type implements ienumerable, but is not an ilist. allow deferred enumeration
                        bindExpression = Expression.Bind(destinationMember, selectExpression);
                    }
                }
                else if (result.Type != propertyMap.DestinationPropertyType &&
                    // avoid nullable etc.
                         propertyMap.DestinationPropertyType.BaseType != typeof(ValueType) &&
                         propertyMap.DestinationPropertyType.BaseType != typeof(Enum))
                {
                    var transformedExpression = CreateMapExpression(mappingEngine, result.Type,
                                                                    propertyMap.DestinationPropertyType,
                                                                    result.ResolutionExpression);

                    bindExpression = Expression.Bind(destinationMember, transformedExpression);
                }
                else
                {
                    throw new AutoMapperMappingException("Unable to create a map expression from " + result.Type + " to " + propertyMap.DestinationPropertyType);
                }

                bindings.Add(bindExpression);
            }
            return bindings;
        }

        private static Type GetDestinationListTypeFor(PropertyMap propertyMap)
        {
            Type destinationListType;
            if (propertyMap.DestinationPropertyType.IsArray)
                destinationListType = propertyMap.DestinationPropertyType.GetElementType();
            else
                destinationListType = propertyMap.DestinationPropertyType.GetGenericArguments().First();
            return destinationListType;
        }

        private static MethodCallExpression GetToListCallExpression(PropertyMap propertyMap, Type destinationListType, MethodCallExpression selectExpression)
        {
            return Expression.Call(
                typeof(Enumerable),
                propertyMap.DestinationPropertyType.IsArray ? "ToArray" : "ToList",
                new Type[] { destinationListType },
                selectExpression);
        }
    }

    /// <summary>
    /// Continuation to execute projection
    /// </summary>
    public interface IProjectionExpression
    {
        /// <summary>
        /// Projects the source type to the destination type given the mapping configuration
        /// </summary>
        /// <typeparam name="TResult">Destination type to map to</typeparam>
        /// <returns>Queryable result, use queryable extension methods to project and execute result</returns>
        IQueryable<TResult> To<TResult>();
    }

    public class ProjectionExpression<TSource> :IProjectionExpression
    {
        private readonly IQueryable<TSource> _source;
        private readonly IMappingEngine _mappingEngine;

        public ProjectionExpression(IQueryable<TSource> source, IMappingEngine mappingEngine)
        {
            _source = source;
            _mappingEngine = mappingEngine;
        }

        public IQueryable<TResult> To<TResult>()
        {
            Expression<Func<TSource, TResult>> expr = _mappingEngine.CreateMapExpression<TSource, TResult>();

            return _source.Select(expr);
        }
    }

    public class InterfaceConstructorInfo<T> :ConstructorInfo
    {
        private readonly ConstructorInfo _info;
        private readonly Type _interfaceType;
        public InterfaceConstructorInfo(Type concreteType)
            : base()
        {
            _info = concreteType.GetConstructor(new Type[0]);
            _interfaceType = typeof(T);
        }

        public override object Invoke(BindingFlags invokeAttr, Binder binder, object[] parameters, System.Globalization.CultureInfo culture)
        {
            return (T)_info.Invoke(invokeAttr, binder, parameters, culture);
        }

        public override MethodAttributes Attributes
        {
            get { return _info.Attributes; }
        }

        public override MethodImplAttributes GetMethodImplementationFlags()
        {
            return _info.GetMethodImplementationFlags();
        }

        public override ParameterInfo[] GetParameters()
        {
            return _info.GetParameters();
        }

        public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, System.Globalization.CultureInfo culture)
        {
            return (T)_info.Invoke(obj, invokeAttr, binder, parameters, culture);
        }

        public override RuntimeMethodHandle MethodHandle
        {
            get { return _info.MethodHandle; }
        }

        public override Type DeclaringType
        {
            get { return _interfaceType; }
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return _info.GetCustomAttributes(attributeType, inherit);
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            return _info.GetCustomAttributes(inherit);
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return _info.IsDefined(attributeType, inherit);
        }

        public override string Name
        {
            get { return _interfaceType.Name; }
        }

        public override Type ReflectedType
        {
            get { return _info.ReflectedType; }
        }
    }

    public class InterfaceType :Type
    {
        private readonly Type _interfaceType;
        private readonly Type _concreteType;
        public InterfaceType(Type interfaceType, Type concreteType)
            :base()
        {
            _interfaceType = interfaceType;
            _concreteType = concreteType;
        }

        public override Assembly Assembly
        {
            get { return _interfaceType.Assembly; }
        }

        public override string AssemblyQualifiedName
        {
            get { return _interfaceType.AssemblyQualifiedName; }
        }

        public override Type BaseType
        {
            get { return _concreteType; }
        }

        public override string FullName
        {
            get { return _interfaceType.FullName; }
        }

        public override Guid GUID
        {
            get { return _interfaceType.GUID; }
        }

        protected override TypeAttributes GetAttributeFlagsImpl()
        {
            throw new NotImplementedException();
        }

        protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
        {
            return _concreteType.GetConstructor(bindingAttr, binder, callConvention, types, modifiers);
        }

        public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
        {
            return _concreteType.GetConstructors(bindingAttr);
        }

        public override Type GetElementType()
        {
            return _interfaceType.GetElementType();
        }

        public override EventInfo GetEvent(string name, BindingFlags bindingAttr)
        {
            return _concreteType.GetEvent(name, bindingAttr);
        }

        public override EventInfo[] GetEvents(BindingFlags bindingAttr)
        {
            return _concreteType.GetEvents(bindingAttr);
        }

        public override FieldInfo GetField(string name, BindingFlags bindingAttr)
        {
            return _concreteType.GetField(name, bindingAttr);
        }

        public override FieldInfo[] GetFields(BindingFlags bindingAttr)
        {
            return _concreteType.GetFields(bindingAttr);
        }

        public override Type GetInterface(string name, bool ignoreCase)
        {
            return _interfaceType.GetInterface(name, ignoreCase);
        }

        public override Type[] GetInterfaces()
        {
            return _interfaceType.GetInterfaces();
        }

        public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
        {
            return _concreteType.GetMembers(bindingAttr);
        }

        protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
        {
            return _concreteType.GetMethod(name, bindingAttr, binder, callConvention, types, modifiers);
        }

        public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
        {
            return _concreteType.GetMethods(bindingAttr);
        }

        public override Type GetNestedType(string name, BindingFlags bindingAttr)
        {
            return _interfaceType.GetNestedType(name, bindingAttr);
        }

        public override Type[] GetNestedTypes(BindingFlags bindingAttr)
        {
            return _interfaceType.GetNestedTypes(bindingAttr);
        }

        public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
        {
            return _concreteType.GetProperties(bindingAttr);
        }

        protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
        {
            return _concreteType.GetProperty(name, bindingAttr, binder, returnType, types, modifiers);
        }

        protected override bool HasElementTypeImpl()
        {
            return _concreteType.HasElementType;
        }

        public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, System.Globalization.CultureInfo culture, string[] namedParameters)
        {
            return _concreteType.InvokeMember(name, invokeAttr, binder, target, args, culture);
        }

        protected override bool IsArrayImpl()
        {
            return _interfaceType.IsArray;
        }

        protected override bool IsByRefImpl()
        {
            return _concreteType.IsByRef;
        }

        protected override bool IsCOMObjectImpl()
        {
            return _concreteType.IsCOMObject;
        }

        protected override bool IsPointerImpl()
        {
            return _concreteType.IsPointer;
        }

        protected override bool IsPrimitiveImpl()
        {
            return _interfaceType.IsPrimitive;
        }

        public override Module Module
        {
            get { return _interfaceType.Module; }
        }

        public override string Namespace
        {
            get { return _interfaceType.Namespace; }
        }

        public override Type UnderlyingSystemType
        {
            get { return _interfaceType.UnderlyingSystemType; }
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return _concreteType.GetCustomAttributes(attributeType, inherit);
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            return _concreteType.GetCustomAttributes(inherit);
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return _concreteType.IsDefined(attributeType, inherit);
        }

        public override string Name
        {
            get { return _interfaceType.Name; }
        }
    }
}