/****** Object:  Table [dbo].[Tenant]    Script Date: 03/09/2025 23:21:37 ******/
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Tenant' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [dbo].[Tenant](
        [id] [bigint] IDENTITY(1,1) NOT NULL,
        [tenantname] [nvarchar](50) NULL,
        [active] [bit] NULL,
        [subdomain] [nvarchar](50) NULL,
    CONSTRAINT [PK_Tenant] PRIMARY KEY CLUSTERED 
    (
        [id] ASC
    )WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    ) ON [PRIMARY]

    insert into tenant (tenantname, active, subdomain)
    values

    ('Administrators', 1, 'admin'),
    ('Company 1', 1, 'company1'),
    ('Company 2', 1, 'company2'),
    ('Company 3', 1, 'company3'),
    ('Company 4', 1, 'company4'),
    ('Company 5', 1, 'company5')
END
