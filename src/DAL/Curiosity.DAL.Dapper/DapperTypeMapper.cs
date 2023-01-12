using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using Dapper;

namespace Curiosity.DAL.Dapper
{
    /// <summary>
    /// Mapper for Dapper query execution
    /// </summary>
    /// <remarks>
    /// To get entity from dapper query you need to register type for Dapper
    /// using <see cref="RegisterTypes"/>
    /// </remarks>
    public class DapperTypeMapper : SqlMapper.ITypeMap
    {
        private readonly SqlMapper.ITypeMap _internalMapper;

        /// <summary>
        /// Register type for executing queries via Dapper.
        /// </summary>
        /// <param name="types"></param>
        public static void RegisterTypes(params Type[] types)
        {
            for (var i = 0; i < types.Length; i++)
            {
                var type = types[i];
                SqlMapper.SetTypeMap(type, new DapperTypeMapper(type));
            }
        }

        public DapperTypeMapper(Type type)
        {
            _internalMapper = new CustomPropertyTypeMap(type, _propertyResolver);
        }

        public ConstructorInfo FindConstructor(string[] names, Type[] types)
        {
            return _internalMapper.FindConstructor(names, types);
        }

        public ConstructorInfo FindExplicitConstructor()
        {
            return _internalMapper.FindExplicitConstructor();
        }

        public SqlMapper.IMemberMap GetConstructorParameter(ConstructorInfo constructor, string columnName)
        {
            return _internalMapper.GetConstructorParameter(constructor, columnName);
        }

        /// <summary>
        /// Returns mapping member for specified column name.
        /// </summary>
        /// <param name="columnName">Column name</param>
        /// <returns></returns>
        public SqlMapper.IMemberMap GetMember(string columnName)
        {
            return _internalMapper.GetMember(columnName);
        }

        private readonly Func<Type, string, PropertyInfo?> _propertyResolver = (type, name) =>
        {
            var properties =
                type.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic |
                                   BindingFlags.Public);

            for (var i = 0; i < properties.Length; i++)
            {
                var property = properties[i];
                if (property.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return property;
                }

                var attribute = property.GetCustomAttribute<ColumnAttribute>();

                if (attribute?.Name?.Equals(name, StringComparison.OrdinalIgnoreCase) ?? false)
                {
                    return property;
                }
            }

            return null;
        };
    }
}