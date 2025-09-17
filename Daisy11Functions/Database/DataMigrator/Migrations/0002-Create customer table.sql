/****** Object:  Table [dbo].[Customer]    Script Date: 03/09/2025 23:21:06 ******/
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Customer' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [dbo].[Customer](
        [id] [bigint] IDENTITY(1,1) NOT NULL,
        [vehicle] [nvarchar](50) NULL,
        [increasedate] [nvarchar](50) NULL,
        [fineoperator] [nvarchar](50) NULL,
        [fineamount] [decimal](18, 5) NULL,
        [age] [decimal](18, 5) NULL,
        [power] [decimal](18, 5) NULL,
        [issuer] [nvarchar](50) NULL,
        [status] [nvarchar](50) NULL,
    CONSTRAINT [PK_Customer] PRIMARY KEY CLUSTERED 
    (
        [id] ASC
    )WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    ) ON [PRIMARY]

	insert into customer (vehicle, increasedate, fineoperator, fineamount, age, power, issuer, status)
	values 

	('GF57 YHT', '20250406T1500', 'BT', 34, 3, 34,  'External' , 'To load'),
	('FG25 GHL', '20250407T1500', 'BT', 54, 3, 98,  'Internal' , 'Complete'),
	('SD23 SHP', '20250408T1500', 'ER', 76, 4, 32,  'External' , 'Processing'),
	('GF42 JDJ', '20250411T1500', 'BT', 32, 6, 82,  'External' , 'Complete'),
	('LF87 RWO', '20250432T1500', 'GH', 36, 8, 65,  'Internal' , 'To load'),
	('GW57 UST', '20250406T1500', 'GH', 14, 1, 34,  'External' , 'To load'),
	('DJ43 OZY', '20250407T1500', 'DH', 24, 9, 90,  'Internal' , 'Complete'),
	('SH56 PME', '20250408T1500', 'YJ', 46, 2, 34,  'External' , 'Processing'),
	('GR19 KUY', '20250411T1500', 'PW', 62, 3, 81,  'External' , 'Complete'),
	('LG29 GRO', '20250432T1500', 'CE', 86, 4, 60,  'Internal' , 'To load')
END
