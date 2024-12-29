using System.Text.Json;

namespace MinimalAPI;

public class Parser
{
    public static IReadOnlyCollection<KeyValuePair<string, string>> Parse()
    {
        var doc = JsonDocument.Parse(AllSettings);
        var root = doc.RootElement;
        var kvp = new List<KeyValuePair<string, string>>();
        ParseJsonElement(root, kvp);
        return kvp;
    }

    private static void ParseJsonElement(JsonElement element, List<KeyValuePair<string, string>> kvp, string path = "")
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            foreach (var property in element.EnumerateObject())
            {
                var propertyPath = String.IsNullOrEmpty(path) ? property.Name : $"{path}:{property.Name}";
                ParseJsonElement(property.Value, kvp, propertyPath);
            }

            return;
        }

        if (element.ValueKind == JsonValueKind.Array)
        {
            var arrayItems = element.GetArrayLength();
            for (int i = 0; i < arrayItems; i++)
            {
                ParseJsonElement(element[i], kvp, $"{path}:{i}");
            }

            return;
        }

        kvp.Add(new KeyValuePair<string, string>(path, element.ToString()));
    }


    private static string Serilog = """
                                    {
                                      "Description": "Serilog setup",
                                      "WriteTo": [
                                        {
                                          "Name": "RabbitMQ",
                                          "Args": {
                                            "ClientConfiguration": {
                                              "Hostnames": [
                                                "127.0.0.1"
                                              ],
                                              "Port": 5672,
                                              "Username": "guest",
                                              "Password": "guest",
                                              "Exchange": "cronus_apps",
                                              "ExchangeType": "fanout",
                                              "DeliveryMode": "Durable",
                                              "RouteKey": "#"
                                            },
                                            "SinkConfiguration": {
                                              "BatchPostingLimit": 16
                                            },
                                            "TextFormatter": "Cronus.ContentService.Web.Infrastructure.SerilogGelfJsonFormatter.GelfJsonFormatter,Cronus.ContentService.Web"
                                          }
                                        },
                                        {
                                          "Name": "Console",
                                          "Args": {
                                            "restrictedToMinimumLevel": "Debug"
                                          }
                                        }
                                      ],
                                      "Enrich": [
                                        "FromLogContext",
                                        "WithMachineName",
                                        "WithProcessName",
                                        "WithMemoryUsage"
                                      ],
                                      "Properties": {
                                        "host": "Cronus.ContentService",
                                        "facility": "Cronus.ContentService",
                                        "is_cronus": 1
                                      },
                                      "Using": [
                                        "Serilog.Expressions"
                                      ],
                                      "Filter": [
                                        {
                                          "Name": "ByExcluding",
                                          "Args": {
                                            "expression": "RequestPath IN ['/metrics', '/diagnostics/liveness', '/diagnostics/readiness'] AND StatusCode = 200"
                                          }
                                        }
                                      ],
                                      "MinimumLevel": {
                                        "Default": "Debug",
                                        "Override": {
                                          "Microsoft.AspNetCore.Routing.EndpointMiddleware": "Warning",
                                          "Microsoft.AspNetCore.Hosting.Diagnostics": "Warning",
                                          "Microsoft.AspNetCore.Hosting": "Information",
                                          "Microsoft.AspNetCore.Routing": "Warning",
                                          "Microsoft.AspNetCore.Server.Kestrel": "Information",
                                          "Microsoft.AspNetCore.Mvc.Internal": "Warning",
                                          "Microsoft.AspNetCore.Mvc": "Warning",
                                          "Microsoft": "Information",
                                          "Microsoft.AspNetCore.Cors.Infrastructure.CorsService": "Warning",
                                          "System": "Information",
                                          "MicroElements.Swashbuckle.FluentValidation": "Information"
                                        }
                                      }
                                    }
                                    """;

    private static string AllSettings = """
                                        {
                                            "ThreadPoolSize": 200,
                                            "Kestrel": {
                                                "Endpoints": {
                                                    "Http": {
                                                        "Url": "http://0.0.0.0:15000"
                                                    }
                                                }
                                            },
                                            "Serilog": {
                                                "WriteTo": [
                                                    {
                                                        "Name": "RabbitMQ",
                                                        "Args": {
                                                            "ClientConfiguration": {
                                                                "Hostnames": [
                                                                    "127.0.0.1"
                                                                ],
                                                                "Port": 5672,
                                                                "Username": "guest",
                                                                "Password": "guest",
                                                                "Exchange": "cronus_apps",
                                                                "ExchangeType": "fanout",
                                                                "DeliveryMode": "Durable",
                                                                "RouteKey": "#"
                                                            },
                                                            "SinkConfiguration": {
                                                                "BatchPostingLimit": 16
                                                            },
                                                            "TextFormatter": "Cronus.ContentService.Web.Infrastructure.SerilogGelfJsonFormatter.GelfJsonFormatter,Cronus.ContentService.Web"
                                                        }
                                                    },
                                                    {
                                                        "Name": "Console",
                                                        "Args": {
                                                            "restrictedToMinimumLevel": "Debug"
                                                        }
                                                    }
                                                ],
                                                "Enrich": [
                                                    "FromLogContext",
                                                    "WithMachineName",
                                                    "WithProcessName",
                                                    "WithMemoryUsage"
                                                ],
                                                "Properties": {
                                                    "host": "Cronus.ContentService",
                                                    "facility": "Cronus.ContentService",
                                                    "is_cronus": 1
                                                },
                                                "Using": [
                                                    "Serilog.Expressions"
                                                ],
                                                "Filter": [
                                                    {
                                                        "Name": "ByExcluding",
                                                        "Args": {
                                                            "expression": "RequestPath IN ['/metrics', '/diagnostics/liveness', '/diagnostics/readiness'] AND StatusCode = 200"
                                                        }
                                                    }
                                                ],
                                                "MinimumLevel": {
                                                    "Default": "Debug",
                                                    "Override": {
                                                        "Microsoft.AspNetCore.Routing.EndpointMiddleware": "Warning",
                                                        "Microsoft.AspNetCore.Hosting.Diagnostics": "Warning",
                                                        "Microsoft.AspNetCore.Hosting": "Information",
                                                        "Microsoft.AspNetCore.Routing": "Warning",
                                                        "Microsoft.AspNetCore.Server.Kestrel": "Information",
                                                        "Microsoft.AspNetCore.Mvc.Internal": "Warning",
                                                        "Microsoft.AspNetCore.Mvc": "Warning",
                                                        "Microsoft": "Information",
                                                        "Microsoft.AspNetCore.Cors.Infrastructure.CorsService": "Warning",
                                                        "System": "Information",
                                                        "MicroElements.Swashbuckle.FluentValidation": "Information"
                                                    }
                                                }
                                            },
                                            "Telemetry": {
                                                "Host": "localhost",
                                                "Port": 6831,
                                                "Enabled": true
                                            },
                                            "ConnectionStrings": {
                                                "CronusBackofficeDB": "Data Source=127.0.0.1,1433; uid=sa;pwd=Some_password123; TrustServerCertificate=True; Initial Catalog=CronusBackofficeDB",
                                                "OnjnSafeServerDb": "Data Source=tcp:STG_vOperDBRO_Ad.stgkaizen.local,1433; Persist Security Info=False;Integrated Security=true; Initial Catalog=ONJN_Safe_Server; TrustServerCertificate=true",
                                                "CasinoConfigDB": "Data Source=STG_SiteConfig2.stgkaizen.local,1433; Persist Security Info=False; uid=Tester;pwd=4O3Ts58C1S44w7J; Initial Catalog=CasinoConfigDB; TrustServerCertificate=true"
                                            },
                                            "JwtOptions": {
                                                "SecurityKey": "This should be a strong key [min 32 chars]",
                                                "TokenLifeTime": "12:45:00"
                                            },
                                            "Policies": {
                                                "default": "http://localhost:4200",
                                                "experiment": "127.0.0.1:4200"
                                            },
                                            "LdapConnection": {
                                                "FullyQualifiedDomainName": "stgkaizen.local",
                                                "DomainName": "stgkaizen.local",
                                                "DomainFolder": "stgkaizen"
                                            },
                                            "Configurator": {
                                                "ApiUrl": "http://configurator-host-webapi.apps.stgocp.stgkaizen.local",
                                                "RabbitMqHost": "rabbitmq.stgkaizen.local",
                                                "ApplicationName": "Gaming.BackOffice",
                                                "FileOverridesEnabled": true,
                                                "RabbitMqUsername": "guest",
                                                "RabbitMqPassword": "guest",
                                                "RabbitMqPort": 5672,
                                                "RabbitMqVirtualHost": "configurator",
                                                "RabbitMqConsumerQueuePrefix": "configurator_out_",
                                                "RabbitMqConsumerExchange": "configurator_out",
                                                "RabbitMqProducerExchange": "configurator_in"
                                            },
                                            "CronusConfigurator": {
                                                "ApiUrl": "http://configurator-host-cronus-webapi.apps.stgocp.stgkaizen.local/api/sdk",
                                                "RabbitMqHost": "stg-secpocarmq.stgkaizen.local",
                                                "ApplicationName": "Gaming.BackOffice",
                                                "FileOverridesEnabled": true,
                                                "RabbitMqUsername": "configurator_cronus",
                                                "RabbitMqPassword": "configurator_cronus",
                                                "RabbitMqPort": 5675,
                                                "RabbitMqVirtualHost": "cronus",
                                                "RabbitMqConsumerQueuePrefix": "configurator_out_",
                                                "RabbitMqConsumerExchange": "configurator_out",
                                                "RabbitMqProducerExchange": "configurator_in"
                                            },
                                            "SafeServersSetup": {
                                                "Regulators": [
                                                    {
                                                        "id": 1,
                                                        "name": "HGC-GR",
                                                        "operatorId": 1,
                                                        "productTypes": [
                                                            1,
                                                            2,
                                                            3
                                                        ],
                                                        "requiredProperties": [
                                                            "CategoryId",
                                                            "GameTypeId",
                                                            "CertNumber"
                                                        ]
                                                    },
                                                    {
                                                        "id": 2,
                                                        "name": "ONJN-RO",
                                                        "operatorId": 4,
                                                        "productTypes": [
                                                            1,
                                                            2
                                                        ],
                                                        "requiredProperties": [
                                                            "CategoryId",
                                                            "CertNumber"
                                                        ]
                                                    },
                                                    {
                                                        "id": 3,
                                                        "name": "MOF-CZ",
                                                        "operatorId": 8,
                                                        "productTypes": [
                                                            1,
                                                            2
                                                        ],
                                                        "requiredProperties": [
                                                            "CategoryId",
                                                            "MinBet",
                                                            "MaxBet",
                                                            "CertNumber"
                                                        ]
                                                    },
                                                    {
                                                        "id": 4,
                                                        "name": "SRIJ-PT",
                                                        "operatorId": 9,
                                                        "productTypes": [
                                                            1,
                                                            2
                                                        ],
                                                        "requiredProperties": [
                                                            "CategoryId",
                                                            "CertNumber"
                                                        ]
                                                    },
                                                    {
                                                        "id": 5,
                                                        "name": "LUGAS-DE",
                                                        "operatorId": 11,
                                                        "productTypes": [
                                                            1,
                                                            2
                                                        ],
                                                        "requiredProperties": [
                                                            "CategoryId",
                                                            "MinBet",
                                                            "MaxBet",
                                                            "ProfitRatio",
                                                            "CertNumber"
                                                        ]
                                                    },
                                                    {
                                                        "id": 6,
                                                        "name": "NRA-BG",
                                                        "operatorId": 16,
                                                        "productTypes": [
                                                            1,
                                                            2
                                                        ],
                                                        "requiredProperties": [
                                                            "CategoryId",
                                                            "CertNumber"
                                                        ]
                                                    },
                                                    {
                                                        "id": 7,
                                                        "name": "CA-ON",
                                                        "operatorId": 20,
                                                        "productTypes": [
                                                            1,
                                                            2
                                                        ],
                                                        "requiredProperties": [
                                                            "CategoryId",
                                                            "GameTypeId",
                                                            "CertNumber",
                                                            "CertItl",
                                                            "FilesList",
                                                            "HashList",
                                                            "SupplierRegNumber"
                                                        ]
                                                    },
                                                    {
                                                        "id": 8,
                                                        "name": "Coljuegos - CO",
                                                        "operatorId": 23,
                                                        "productTypes": [
                                                            1,
                                                            2
                                                        ],
                                                        "requiredProperties": [
                                                            "CategoryId",
                                                            "CertNumber"
                                                        ]
                                                    },
                                                    {
                                                        "id": 9,
                                                        "name": "MX-Azteca",
                                                        "operatorId": 24,
                                                        "productTypes": [ 1, 2, 4 ],
                                                        "requiredProperties": [
                                                            "CategoryId",
                                                            "CertNumber"
                                                        ]
                                                    },
                                                    {
                                                        "id": 10,
                                                        "name": "AR - Buenos Aires Province",
                                                        "operatorId": 25,
                                                        "productTypes": [
                                                            1,
                                                            2
                                                        ],
                                                        "requiredProperties": [
                                                            "CategoryId",
                                                            "CertNumber"
                                                        ]
                                                    },
                                                    {
                                                        "id": 11,
                                                        "name": "Mincetur - PE",
                                                        "operatorId": 18,
                                                        "productTypes": [ 1, 2, 3 ],
                                                        "requiredProperties": [
                                                            "CategoryId",
                                                            "ProfitRatio",
                                                            "CertNumber"
                                                        ]
                                                    }
                                                ]
                                            },
                                            "SftpOptionsSettings": {
                                                "Default": {
                                                    "Host": "LIN-STG-FTP01.stgkaizen.local",
                                                    "Port": 22,
                                                    "UserName": "sftp-casino-static",
                                                    "Password": "f#R8e$KJu5%2Ni",
                                                    "WorkingDirectory": "/tmp"
                                                },
                                                "Virtuals": {
                                                    "Host": "LIN-STG-FTP01.stgkaizen.local",
                                                    "Port": 22,
                                                    "UserName": "sftp-casino-static",
                                                    "Password": "f#R8e$KJu5%2Ni",
                                                    "WorkingDirectory": "/tmp"
                                                }
                                            },
                                            "LugasPublisherSettings": {
                                                "Enabled": true,
                                                "Host": "localhost",
                                                "Port": "5672",
                                                "Username": "guest",
                                                "Password": "guest",
                                                "ExchangeName": "backoffice.casinosafeservergame",
                                                "QueueSize": 20000
                                            },
                                            "SdkApiKeys": [
                                                {
                                                    "ApiKey": "D00385AA-22C1-42BF-BE53-57047550E08A",
                                                    "AppName": "CasinoV3",
                                                    "Enabled": true,
                                                    "Roles": [
                                                        "ReadAll"
                                                    ]
                                                },
                                                {
                                                    "ApiKey": "6983B978-4385-402B-9767-9D38DDE2790F",
                                                    "AppName": "NAPI",
                                                    "Enabled": true,
                                                    "Roles": [
                                                        "ReadAll"
                                                    ]
                                                },
                                                {
                                                    "ApiKey": "3E487ED3-6079-44BD-9D4A-0F161BBC204A",
                                                    "AppName": "MyAccount",
                                                    "Enabled": true,
                                                    "Roles": [
                                                        "ReadAll"
                                                    ]
                                                },
                                                {
                                                    "ApiKey": "E31907ED-6CB7-48DD-A650-D8F742DE497E",
                                                    "AppName": "DamaoGlobal.Admin",
                                                    "Enabled": true,
                                                    "Roles": [
                                                        "ReadAll"
                                                    ]
                                                },
                                                {
                                                    "ApiKey": "41497CDF-19AE-491C-BD93-08453D97269F",
                                                    "AppName": "Pandora.Assistant",
                                                    "Enabled": true,
                                                    "Roles": [
                                                        "ReadAll"
                                                    ]
                                                },
                                                {
                                                    "ApiKey": "07AC7FFE-4488-4681-AF7C-ADA0C92E4CBB",
                                                    "AppName": "DamaoGlobal.Business",
                                                    "Enabled": true,
                                                    "Roles": [
                                                        "ReadAll"
                                                    ]
                                                },
                                                {
                                                    "ApiKey": "C4AC6724-64A6-4717-8B10-263E24EE4E44",
                                                    "AppName": "LeapGames.Web",
                                                    "Enabled": true,
                                                    "Roles": [
                                                        "ReadAll"
                                                    ]
                                                },
                                                {
                                                    "ApiKey": "D560E2FA-C827-431B-85C7-DB5BC818B8F6",
                                                    "AppName": "Gaming.GameLauncher.Service",
                                                    "Enabled": true,
                                                    "Roles": [
                                                        "ReadAll"
                                                    ]
                                                },
                                                {
                                                    "ApiKey": "905ECE58-0686-4255-8B84-1E49D30DE5F1",
                                                    "AppName": "Vaix.Tools.FileTransfer",
                                                    "Enabled": true,
                                                    "Roles": [
                                                        "ReadAll"
                                                    ]
                                                },
                                                {
                                                    "ApiKey": "6B0D0439-43F9-4F2A-8BA9-59E6EB5B93A6",
                                                    "AppName": "Damao_NetEXE",
                                                    "Enabled": true,
                                                    "Roles": [
                                                        "ReadAll"
                                                    ]
                                                },
                                                {
                                                    "ApiKey": "BE91FC83-A475-48BA-9914-9C502A5A76F0",
                                                    "AppName": "SportsbookV3",
                                                    "Enabled": true,
                                                    "Roles": [
                                                        "ReadAll"
                                                    ]
                                                },
                                                {
                                                    "ApiKey": "3E89BF27-5254-4595-9882-AA95265F22E9",
                                                    "AppName": "MyAccount.WebApi",
                                                    "Enabled": true,
                                                    "Roles": [
                                                        "ReadAll"
                                                    ]
                                                },
                                                {
                                                    "ApiKey": "3A4F5A0D-B5A5-462D-94B1-90B319BB963A",
                                                    "AppName": "Gaming.Backoffice.Facade",
                                                    "Enabled": true,
                                                    "Roles": [
                                                        "ReadAll"
                                                    ]
                                                },
                                                {
                                                    "ApiKey": "13941170-B4F8-4995-9450-CBB83559EE7D",
                                                    "AppName": "Casino.RecommendationEngine.Tasks",
                                                    "Enabled": true,
                                                    "Roles": [
                                                        "WriteGlobalRecommendation"
                                                    ]
                                                },
                                                {
                                                    "ApiKey": "F0B225FD-02E5-4CE5-9105-F7B406CCF9E6",
                                                    "AppName": "Cronus.Data.Feed",
                                                    "Enabled": true,
                                                    "Roles": [
                                                        "WriteLiveTableGames"
                                                    ]
                                                },
                                                {
                                                    "ApiKey": "A5430383-5FBF-44EF-93CC-521C5CD7DABF",
                                                    "AppName": "Kaizen.Pyrrha",
                                                    "Enabled": true,
                                                    "Roles": [ "ReadAll" ]
                                                }
                                            ],
                                            "UsersWhiteList": [
                                                "giorgos",
                                                "admin",
                                                "test"
                                            ],
                                            "ValidImageContentTypes": [
                                                {
                                                    "AssetTypeName": "CategoryIcon",
                                                    "ContentTypes": [ "image/jpeg", "image/svg+xml", "application/json" ]
                                                },
                                                {
                                                    "AssetTypeName": "CategoryBackground",
                                                    "ContentTypes": [ "image/jpeg", "image/png" ]
                                                },
                                                {
                                                    "AssetTypeName": "CategoryAvatar",
                                                    "ContentTypes": [ "image/jpeg", "image/png" ]
                                                },
                                                {
                                                    "AssetTypeName": "GameMain",
                                                    "ContentTypes": [ "image/jpeg", "image/svg+xml", "image/png", "image/webp", "image/x-icon", "image/gif" ]
                                                },
                                                {
                                                    "AssetTypeName": "GameThumbnail",
                                                    "ContentTypes": [ "image/jpeg", "image/svg+xml", "image/png", "image/webp", "image/x-icon", "image/gif" ]
                                                },
                                                {
                                                    "AssetTypeName": "GameBackground",
                                                    "ContentTypes": [ "image/jpeg", "image/svg+xml", "image/png" ]
                                                },
                                                {
                                                    "AssetTypeName": "GameLarge",
                                                    "ContentTypes": [ "image/jpeg", "image/svg+xml", "image/png", "image/x-icon" ]
                                                },
                                                {
                                                    "AssetTypeName": "GameMini",
                                                    "ContentTypes": [ "image/jpeg", "image/svg+xml", "image/png" ]
                                                },
                                                {
                                                    "AssetTypeName": "GameDealer",
                                                    "ContentTypes": [ "image/jpeg", "image/svg+xml", "image/png", "image/webp", "image/x-icon", "image/gif" ]
                                                },
                                                {
                                                    "AssetTypeName": "GameLanguage",
                                                    "ContentTypes": [ "image/jpeg", "image/svg+xml", "image/png", "image/webp", "image/x-icon", "image/gif" ]
                                                }
                                            ],
                                            "DefaultAssetPrefixPath": "https://develop-backoffice-staging.stoiximan.gr/casino/",
                                            "VirtualsAssetPrefixPath": "https://develop-v3-staging-q5i8zrav.stoiximan.gr/static/virtuals/games/images/"
                                        }

                                        """;
}