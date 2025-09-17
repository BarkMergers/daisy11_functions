/****** Object:  Table [dbo].[Inventory]    Script Date: 12/09/2025 23:20:49 *****/
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Inventory' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [dbo].Inventory(
        [id] [bigint] IDENTITY(1,1) NOT NULL,
        [name] [nvarchar](50) NULL,
        [description] [nvarchar](50) NULL,
        [quantity] [bigint] NULL,
    CONSTRAINT [PK_Inventory] PRIMARY KEY CLUSTERED 
    (
        [id] ASC
    )WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    ) ON [PRIMARY]
END
