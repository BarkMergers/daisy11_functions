/****** Object:  Table [dbo].[Agent]    Script Date: 03/09/2025 23:20:49 ******/
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Agent' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [dbo].[Agent](
        [id] [bigint] IDENTITY(1,1) NOT NULL,
        [agent] [nvarchar](50) NULL,
        [role] [nvarchar](50) NULL,
        [tenantid] [bigint] NULL,
        [active] [bit] NULL,
        [firstname] [nvarchar](50) NULL,
        [lastname] [nvarchar](50) NULL,
        [age] [int] NULL,
    CONSTRAINT [PK_Agent] PRIMARY KEY CLUSTERED 
    (
        [id] ASC
    )WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    ) ON [PRIMARY]
END

