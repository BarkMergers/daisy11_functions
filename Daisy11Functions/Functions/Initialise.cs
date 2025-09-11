using Daisy11Functions.Database.Archive.Tables;
using Daisy11Functions.Database.NewWorld;
using Daisy11Functions.Database.NewWorld.Tables;
using Daisy11Functions.Database.Pagination;
using Daisy11Functions.Helpers;
using Daisy11Functions.Models.FilterAndSort;
using Grpc.Core;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using NewWorldFunctions.Helpers;
using Pipelines.Sockets.Unofficial.Arenas;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Data;
using System;
using System.Runtime.CompilerServices;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Azure;

namespace Daisy11Functions.Functions;







public class Initialise
{
    private  readonly ILogger<Initialise> _logger;
    private readonly INewWorldContext _projectContext;

    public Initialise(ILogger<Initialise> logger, INewWorldContext projectContext)
    {
        _logger = logger;
        _projectContext = projectContext;
    }


    [Function("InitialiseDB")]
    public async Task<HttpResponseData> Run_InitialiseDB([HttpTrigger(AuthorizationLevel.Anonymous, "options", "get", Route = "InitialiseDB")]
        HttpRequestData req)
    {
        if (CORS.IsPreFlight(req, out HttpResponseData response)) return response;

        string sql = GetUpdateScript();

        try
        {
            await _projectContext.ExecuteSqlRawAsync(sql);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Runtime error:");
            return await API.Fail(response, System.Net.HttpStatusCode.BadRequest, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
        }

        return await API.Success(response, "Database initialised");
    }


    private static string GetUpdateScript () {

        string sql = @"IF NOT EXISTS(SELECT 1 FROM sys.tables WHERE name = 'Tenant' AND schema_id = SCHEMA_ID('dbo'))
                        BEGIN

                            CREATE TABLE[dbo].[Tenant](
                                [id][bigint] IDENTITY(1, 1) NOT NULL,
                                [tenantname][nvarchar](50) NULL,
                                [active][bit] NULL,
                                [subdomain][nvarchar](50) NULL,
                                CONSTRAINT[PK_Tenant] PRIMARY KEY CLUSTERED
                            (
                                [id] ASC
                            )WITH(STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON[PRIMARY]
                                ) ON[PRIMARY]

                        END

                        IF NOT EXISTS(SELECT 1 FROM sys.tables WHERE name = 'Agent' AND schema_id = SCHEMA_ID('dbo'))
                        BEGIN

                            CREATE TABLE[dbo].[Agent](
                                [id][bigint] IDENTITY(1, 1) NOT NULL,
                                [agent][nvarchar](50) NULL,
                                [role][nvarchar](50) NULL,
                                [tenantid][bigint] NULL,
                                [active][bit] NULL,
                                [firstname][nvarchar](50) NULL,
                                [lastname][nvarchar](50) NULL,
                                [age][int] NULL,
                                CONSTRAINT[PK_Agent] PRIMARY KEY CLUSTERED
                            (
                                [id] ASC
                            )WITH(STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON[PRIMARY]
                            ) ON[PRIMARY]

                        END

                        IF NOT EXISTS(SELECT 1 FROM sys.tables WHERE name = 'Customer' AND schema_id = SCHEMA_ID('dbo'))
                        BEGIN

                            CREATE TABLE[dbo].[Customer](
                                [id][bigint] IDENTITY(1, 1) NOT NULL,
                                [vehicle][nvarchar](50) NULL,
                                [increasedate][nvarchar](50) NULL,
                                [fineoperator][nvarchar](50) NULL,
                                [fineamount][decimal](18, 5) NULL,
                                [age][decimal](18, 5) NULL,
                                [power][decimal](18, 5) NULL,
                                [issuer][nvarchar](50) NULL,
                                [status][nvarchar](50) NULL,
                                CONSTRAINT[PK_Customer] PRIMARY KEY CLUSTERED
                                (
                                [id] ASC
                            )WITH(STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON[PRIMARY]
                            ) ON[PRIMARY]

                            insert into customer(vehicle, increasedate, fineoperator, fineamount, age, power, issuer, status)

                            values
                            ('GF32 FHG', '2019-01-01T12:30:00', 'EGG', 1000, 3, 76, 'External', 'To load'),
	                        ('JG74 GFD', '2019-01-01T12:30:00', 'HET', 800, 6, 94, 'Internal', 'Complete'),
	                        ('KR74 THA', '2019-01-01T12:30:00', 'BRO', 450, 4, 86, 'External', 'Processing'),
	                        ('KB01 FAQ', '2019-01-01T12:30:00', 'EGG', 900, 1, 68, 'Internal', 'To load'),
	                        ('EV26 RUX', '2019-01-01T12:30:00', 'HET', 1100, 3, 91, 'External', 'Complete')
                        END
                        ";

        return sql;
    }
}