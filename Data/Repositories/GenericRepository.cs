#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Entities.Framework;
using TableAttribute = System.ComponentModel.DataAnnotations.Schema.TableAttribute;

namespace Data.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly IDbConnection _connection;


    public GenericRepository(ApplicationDbContext context)
    {
        _connection = context.CreateConnection();
    }

    public async Task<T?> GetById(int id)
    {
        T? result;
        try
        {
            var tableName = GetTableName();
            var keyColumn = GetKeyColumnName();
            var query = $"SELECT {GetColumnsAsProperties()} FROM {tableName} WHERE {keyColumn} = '{id}'";

            result = await _connection.QueryFirstOrDefaultAsync<T>(query);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching a record from db: ${ex.Message}");
            throw new Exception("Unable to fetch data. Please contact the administrator.");
        }
        finally
        {
            _connection.Close();
        }

        return result;
    }

    public async Task<IEnumerable<T>> GetAll()
    {
        IEnumerable<T> result;
        try
        {
            var tableName = GetTableName();
            var query = $"SELECT {GetColumnsAsProperties()} FROM {tableName}";

            result = await _connection.QueryAsync<T>(query);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching records from db: ${ex.Message}");
            throw new Exception("Unable to fetch data. Please contact the administrator.");
        }
        finally
        {
            _connection.Close();
        }
        return result;
    }

    public async Task<int> CountAll()
    {
        int result;
        try
        {
            var tableName = GetTableName();
            var query = $"SELECT COUNT(*) FROM {tableName}"; // May need exact column names

            result = await _connection.QueryFirstOrDefaultAsync<int>(query);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error counting records in db: ${ex.Message}");
            throw new Exception("Unable to count data. Please contact the administrator.");
        }
        finally
        {
            _connection.Close();
        }

        return result;
    }

    public async Task<int> Add(T entity)
    {
        int rowsEffected;
        try
        {
            var tableName = GetTableName();
            var columns = GetColumns(excludeKey: true);
            var properties = GetPropertyNames(excludeKey: true);
            var query = $"INSERT INTO {tableName} ({columns}) VALUES ({properties})";

            rowsEffected = await _connection.ExecuteAsync(query, entity);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding a record to db: ${ex.Message}");
            rowsEffected = -1;
        }
        finally
        {
            _connection.Close();
        }

        return rowsEffected;
    }

    public async Task<int> Update(T entity)
    {
        int rowsEffected;
        try
        {
            var tableName = GetTableName();
            var keyColumn = GetKeyColumnName();
            var keyProperty = GetKeyPropertyName();

            var query = new StringBuilder();
            query.Append($"UPDATE {tableName} SET ");

            foreach (var property in GetProperties(true))
            {
                var columnAttribute = property.GetCustomAttribute<ColumnAttribute>();

                var propertyName = property.Name;
                var columnName = columnAttribute?.Name ?? "";

                query.Append($"{columnName} = @{propertyName},");
            }

            query.Remove(query.Length - 1, 1);

            query.Append($" WHERE {keyColumn} = @{keyProperty}");

            rowsEffected = await _connection.ExecuteAsync(query.ToString(), entity);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating a record in db: ${ex.Message}");
            rowsEffected = -1;
        }
        finally
        {
            _connection.Close();
        }

        return rowsEffected;
    }

    public async Task<int> Delete(T entity)
    {
        int rowsEffected;
        try
        {
            var tableName = GetTableName();
            var keyColumn = GetKeyColumnName();
            var keyProperty = GetKeyPropertyName();
            var query = $"DELETE FROM {tableName} WHERE {keyColumn} = @{keyProperty}";

            rowsEffected = await _connection.ExecuteAsync(query, entity);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting a record in db: ${ex.Message}");
            rowsEffected = -1;
        }
        finally
        {
            _connection.Close();
        }

        return rowsEffected;
    }

    private static string GetTableName()
    {
        var type = typeof(T);
        var tableAttribute = type.GetCustomAttribute<TableAttribute>();
        return tableAttribute != null ? tableAttribute.Name : type.Name;
    }

    private static string? GetKeyColumnName()
    {
        var properties = typeof(T).GetProperties();

        foreach (var property in properties)
        {
            var keyAttributes = property.GetCustomAttributes(typeof(KeyAttribute), true);

            if (keyAttributes.Length <= 0) continue;
            var columnAttributes = property.GetCustomAttributes(typeof(ColumnAttribute), true);

            if (columnAttributes.Length > 0)
            {
                var columnAttribute = (ColumnAttribute)columnAttributes[0];
                return columnAttribute.Name ?? "";
            }
            else
            {
                return property.Name;
            }
        }

        return null;
    }


    private static string GetColumns(bool excludeKey = false)
    {
        var type = typeof(T);
        var columns = string.Join(", ", type.GetProperties()
            .Where(p => !excludeKey || !p.IsDefined(typeof(KeyAttribute)))
            .Select(p =>
            {
                var columnAttribute = p.GetCustomAttribute<ColumnAttribute>();
                return columnAttribute != null ? columnAttribute.Name : p.Name;
            }));

        return columns;
    }

    private static string GetColumnsAsProperties(bool excludeKey = false)
    {
        var type = typeof(T);
        var columnsAsProperties = string.Join(", ", type.GetProperties()
            .Where(p => !excludeKey || !p.IsDefined(typeof(KeyAttribute)))
            .Select(p =>
            {
                var columnAttribute = p.GetCustomAttribute<ColumnAttribute>();
                return columnAttribute != null ? $"{columnAttribute.Name} AS {p.Name}" : p.Name;
            }));

        return columnsAsProperties;
    }

    private static string GetPropertyNames(bool excludeKey = false)
    {
        var properties = typeof(T).GetProperties()
            .Where(p => !excludeKey || p.GetCustomAttribute<KeyAttribute>() == null);

        var values = string.Join(", ", properties.Select(p => $"@{p.Name}"));

        return values;
    }

    private static IEnumerable<PropertyInfo> GetProperties(bool excludeKey = false)
    {
        var properties = typeof(T).GetProperties()
            .Where(p => !excludeKey || p.GetCustomAttribute<KeyAttribute>() == null);

        return properties;
    }

    private static string? GetKeyPropertyName()
    {
        var properties = typeof(T).GetProperties()
            .Where(p => p.GetCustomAttribute<KeyAttribute>() != null).ToList();

        if (properties.Any())
            return properties.FirstOrDefault()?.Name ?? null;

        return null;
    }
}
public interface IUnit
{
    GenericRepository<T> GetRepository<T>() where T : class, IEntity;
}

public class Unit : IUnit
{
    private readonly ApplicationDbContext _context;

    public Unit(ApplicationDbContext context)
    {
        _context = context;
    }

    public GenericRepository<T> GetRepository<T>() where T : class, IEntity
    {
        return new GenericRepository<T>(_context);
    }
}