using System;
using Dapper;
using Npgsql;

public interface ICalculator
{
    double Add(double x, double y);
    double Subtract(double x, double y);
    double Multiply(double x, double y);
    double Divide(double x, double y);
}

public class Calculator : ICalculator
{
    private string connectionString;
    public NpgsqlDataSource DataSource;

    public Calculator()
    {
        InitializeConnectionString();
        InitializeDatabase();
    }

    private void InitializeConnectionString()
    {
        var rawConnectionString =
            "postgres://qvhvhmbj:88bAt903U_RrV7rdl642k8j_4xk3J_yR@abul.db.elephantsql.com/qvhvhmbj"; //Environment.GetEnvironmentVariable("pgconn");
            var uri = new Uri(rawConnectionString);
            var properlyFormattedConnectionString = string.Format(
                "Server={0};Database={1};User Id={2};Password={3};Port={4};Pooling=false;",
                uri.Host, 
                uri.AbsolutePath.Trim('/'),
                uri.UserInfo.Split(':')[0],
                uri.UserInfo.Split(':')[1],
                uri.Port > 0 ? uri.Port : 5432);
            DataSource =
                new NpgsqlDataSourceBuilder(properlyFormattedConnectionString).Build();
            DataSource.OpenConnection().Close();
    }
    

    private void InitializeDatabase()
    {
            var checkTableCommand = @"
            CREATE TABLE IF NOT EXISTS Calculator (
            id SERIAL PRIMARY KEY,
            x FLOAT NOT NULL,
            y FLOAT NOT NULL,
            Operation VARCHAR(8) NOT NULL,
            Result FLOAT NOT NULL
            );";

            using (var conn = DataSource.OpenConnection())
            {
                    conn.Execute(checkTableCommand);
            }
        }

    public double Add(double x, double y)
    {
        double result = x + y;
        Utilities(x, y, "plus", result);
        return result;
    }

    public double Subtract(double x, double y)
    {
        double result = x - y;
        Utilities(x, y, "minus", result);
        return result;
    }

    public double Multiply(double x, double y)
    {
        double result = x * y;
        Utilities(x, y, "multiply", result);
        return result;
    }

    public double Divide(double x, double y)
    {
        double result = x / y;
        Utilities(x, y, "divide", result);
        return result;
    }

    private void Utilities(double x, double y, string operation, double result)
    {
        var sql =
                @"INSERT INTO Calculator (x, y, Operation, Result) VALUES (@x, @y, @Operation, @Result) RETURNING *;";
            using (var conn = DataSource.OpenConnection())
            {
                conn.QueryFirst<string>(sql, new { x = x, y = y, Operation = operation, Result = result });
            }
    }

    static void Main(string[] args)
    {
        Calculator calculator = new Calculator();
        while (true)
        {
            Console.WriteLine("Welcome. Choose an operation:");
            Console.WriteLine("1. Add");
            Console.WriteLine("2. Subtract");
            Console.WriteLine("3. Multiply");
            Console.WriteLine("4. Divide");
            Console.WriteLine("5. Close");

            string choice = Console.ReadLine();

            if (choice == "5")
            {
                break;
            }

            Console.WriteLine("Enter the first number: ");
            double x = Convert.ToDouble(Console.ReadLine());

            Console.WriteLine("Enter the second number: ");
            double y = Convert.ToDouble(Console.ReadLine());

            double result;
            switch (choice)
            {
                case "1":
                    result = calculator.Add(x, y);
                    break;
                case "2":
                    result = calculator.Subtract(x, y);
                    break;
                case "3":
                    result = calculator.Multiply(x, y);
                    break;
                case "4":
                    result = calculator.Divide(x, y);
                    break;
                default:
                    Console.WriteLine("Invalid character");
                    continue;
            }

            Console.WriteLine($"Result: {result}");
        }
    }
}