CREATE TABLE Role (
    RoleId   INT IDENTITY(1,1) PRIMARY KEY,
    [Name]   NVARCHAR(50) NOT NULL
);

CREATE TABLE Manufacturer (
    ManufacturerId   INT IDENTITY(1,1) PRIMARY KEY,
    [Name]           NVARCHAR(100) NOT NULL
);

CREATE TABLE Category (
    CategoryId   INT IDENTITY(1,1) PRIMARY KEY,
    [Name]       NVARCHAR(100) NOT NULL
);

CREATE TABLE [User](
    UserId      INT IDENTITY(1,1) PRIMARY KEY,
    FullName    NVARCHAR(200) NOT NULL,
    [Login]     NVARCHAR(100) NOT NULL UNIQUE,
    [Password]  NVARCHAR(100) NOT NULL,
    RoleId      INT NOT NULL REFERENCES [Role](RoleId)
);

CREATE TABLE [Product] (
    ProductId      INT IDENTITY(1,1) PRIMARY KEY,
    Article        NVARCHAR(20)  NOT NULL UNIQUE,
    [Name]         NVARCHAR(200) NOT NULL,
    Unit           NVARCHAR(10)  NOT NULL DEFAULT 'шт.',
    Price          DECIMAL(10,2) NOT NULL,
    Author         NVARCHAR(200),
    ManufacturerId INT NOT NULL REFERENCES Manufacturer(ManufacturerId),
    CategoryId     INT NOT NULL REFERENCES Category(CategoryId),
    Discount       INT NOT NULL DEFAULT 0,
    Stock          INT NOT NULL DEFAULT 0,
    [Description]  NVARCHAR(1000),
    Photo          NVARCHAR(100)
);

CREATE TABLE OrderStatus (
    StatusId   INT IDENTITY(1,1) PRIMARY KEY,
    [Name]     NVARCHAR(50) NOT NULL
);

CREATE TABLE OrderFinal (
    OrderId       INT IDENTITY(1,1) PRIMARY KEY,
    OrderNumber   INT NOT NULL UNIQUE,
    OrderDate     DATE NOT NULL,
    DeliveryDate  DATE,
    UserId        INT REFERENCES [User](UserId),
    ReceiptCode   NVARCHAR(10) NOT NULL,
    StatusId      INT NOT NULL REFERENCES OrderStatus(StatusId)
);

CREATE TABLE OrderItem (
    OrderItemId INT IDENTITY(1,1) PRIMARY KEY,
    OrderId     INT NOT NULL REFERENCES OrderFinal(OrderId),
    ProductId   INT NOT NULL REFERENCES [Product](ProductId),
    Quantity    INT NOT NULL DEFAULT 1
);

INSERT INTO [Role]([Name])
VALUES ('Администратор'),('Менеджер'),('Авторизированный клиент');

INSERT INTO [OrderStatus]([Name]) 
VALUES ('Новый'),('В обработке'),('Завершен');

INSERT INTO Manufacturer([Name])
VALUES ('Яуза'),('Т8 Издательские технологии'),('Прогресс книга'),
       ('Время'),('Лениздат'),('Неолит'),('Амрита-Русь'),
       ('Златоуст'),('Аспект Пресс'),('ВКН');

INSERT INTO Category([Name])
VALUES ('Художественная литература'),('Учебник для вузов'),
       ('Хрестоматия'),('Учебное пособие');

INSERT INTO [User](FullName, [Login], [Password], RoleId)
VALUES ('Никифорова Анна Семеновна',       '94d5ous@gmail.com',        'uzWC67', 1),
       ('Стелина Евгения Петровна',        'uth4iz@mail.com',          '2L6KZG', 1),
       ('Михайлюк Анна Вячеславовна',      '5d4zbu@tutanota.com',      'rwVDh9', 1),
       ('Ситдикова Елена Анатольевна',     'ptec8ym@yahoo.com',        'LdNyos', 2),
       ('Ворсин Петр Евгеньевич',          '1qz4kw@mail.com',          'gynQMT', 2),
       ('Старикова Елена Павловна',        '4np6se@mail.com',          'AtnDjr', 2),
       ('Никифорова Весения Николаевна',   'yzls62@outlook.com',       'JlFRCZ', 3),
       ('Сазонов Руслан Германович',       '1diph5e@tutanota.com',     '8ntwUp', 3),
       ('Одинцов Серафим Артёмович',       'tjde7c@yahoo.com',         'YOyhfR', 3),
       ('Степанов Михаил Артёмович',       'wpmrc3do@tutanota.com',    'RSbvHv', 3);

INSERT INTO [Product](Article,[Name],Unit,Price,Author,ManufacturerId,CategoryId,Discount,Stock,[Description],Photo)
VALUES  ('А112Т4', 'Прокляты и убиты','шт.',585,'Виктор Астафьев',1,1,25,6,'Роман-эпопею "Прокляты и убиты" Виктора Астафьева по праву считают одним из самых сильных произведений военной прозы.','1.jpg'),
        ('G843H5', 'Тайны и загадки отца Брауна','шт.',193,'Гилберт Кит Честертон',1,1,30,9,'Гилберт Кит Честертон — признанный классик английской литературы.','2.jpg'),
        ('D325D4', 'Девайс','шт.',1599,'Кирилл Каланджи',2,1,5,12,'Молодой фрилансер Захар Скаро устраивается на подработку — тестирование нового устройства.','3.jpg'),
        ('S432T5', 'Необыкновенное обыкновенное чудо. Школьные истории','шт.',549,'Людмила Улицкая',2,1,15,15,NULL,'4.jpg'),
        ('F325D4', 'Чук и Гек','шт.',209,'Аркадий Гайдар',2,1,18,3,'В книгу вошли повести и рассказы Аркадия Гайдара.','5.jpg'),
        ('G432G6', 'Информационная безопасность. Национальные стандарты Российской Федерации. 3-е издание. Учебное пособие','шт.',3899,'Юрий Родичев',3,2,22,3,'В пособии рассмотрено более 300 документов национальной системы стандартизации.','6.jpg'),
        ('H542F5', 'Linux. Командная строка. Лучшие практики','шт.',1799,'Дэниел Джей Барретт',3,2,4,5,'Перейдите на новый уровень работы в Linux!','7.jpg'),
        ('C346F5', 'Квантовые миры и возникновение пространства-времени','шт.',1349,'Шон Кэрролл',3,2,5,4,'Шон Кэрролл заставляет по-новому взглянуть на физику.','8.jpg'),
        ('F256G6', 'Вселенная. Происхождение жизни, смысл нашего существования и огромный космос','шт.',1799,'Шон Кэрролл',3,2,6,2,'Физик Шон Кэрролл объясняет принципы научных революций.',NULL),
        ('J532V5', 'Пушкин. Бродский. Империя и судьба. В 2-х томах (комплект из 2-х книг)','шт.',529,'Яков Гордин',4,3,8,6,'Первая книга двухтомника "Пушкин. Бродский. Империя и судьба".','10.jpg'),
        ('G643F4', 'Иосиф Бродский. Избранные эссе(комплект из 6-ти книг)','шт.',4925,'Иосиф Бродский',5,3,2,24,'Шесть сборников избранных эссе Иосифа Бродского.','11.jpg'),
        ('J326V5', 'Тысячелетие императорской керамики','шт.',2599,'Янь Чуннянь',5,3,5,4,'Фарфор стал величайшим символом китайской культуры.','12.jpg'),
        ('J632F6', 'Вечные спутники: Портреты из всемирной литературы','шт.',1599,'Дмитрий Мережковский',5,3,0,6,'Цикл критических очерков о культуре и великих литераторах.','13.jpg'),
        ('G632H6', 'Формирование литературной репутации Н.Г.Чернышевского в ХIX-XXI веках','шт.',1349,'Дмитрий Щербаков',6,3,2,8,'Монография о суждениях критиков о Н.Г. Чернышевском.','14.jpg'),
        ('M642E5', 'Теория искусства. Краткий путеводитель','шт.',879,'Роджер Осборн, Дэн Стерджис',6,3,3,2,NULL,'15.jpg'),
        ('G543F5', 'Религиозные верования с древнейших времен до наших дней','шт.',879,'Дмитрий Щербаков',7,3,4,6,'Сборник переводов лекций по истории религий.','16.jpg'),
        ('B653G6', 'Русский язык: Первые шаги. Часть 3. Учебное пособие','шт.',2699,'Любовь Беликова, Инна Ерофеева, Татьяна Шутова',8,4,8,9,'Третья часть учебного комплекса.','17.jpg'),
        ('J735J7', 'Синтетический образ индивидуального психического мира','шт.',1099,'Сергей Моргачев',8,3,9,4,'Психика подобна определённым объектам.','18.jpg'),
        ('H436H7', 'Английский язык в спорте. Учебное пособие','шт.',1999,'Екатерина Габарта, Ирина Игнатьева',9,4,2,0,'Учебное пособие для слушателей языка специальности.','19.jpg'),
        ('H475R5', 'Лексика и грамматика современного китайского языка (к тому II учебника «Новый практический курс китайского языка» под редакцией Лю Сюня): учебное пособие','шт.',608,'Татьяна Лопаткина, Софья Маннапова',10,4,25,12,'Пособие к учебнику "Новый практический курс китайского языка".','20.jpg');

INSERT INTO OrderFinal(OrderNumber,OrderDate,DeliveryDate,UserId,ReceiptCode,StatusId)
VALUES (1, '2025-02-27','2025-04-20',(SELECT UserId FROM [User] WHERE FullName= 'Степанов Михаил Артёмович'),'901',3),
       (2, '2025-03-28','2025-04-21',(SELECT UserId FROM [User] WHERE FullName= 'Никифорова Весения Николаевна'),'789',2),
       (3, '2025-02-20','2026-04-22',NULL,'852',3),
       (4, '2026-03-01','2026-04-23',NULL,'458',2),
       (5, '2026-03-17','2026-04-24',NULL,'905',3),
       (6, '2026-03-21','2026-04-25',(SELECT UserId FROM [User] WHERE FullName= 'Никифорова Весения Николаевна'),'781',3),
       (7, '2026-03-31','2026-04-26',NULL,'128',3),
       (8, '2026-04-02','2026-04-27',(SELECT UserId FROM [User] WHERE FullName= 'Одинцов Серафим Артёмович'),'908',1),
       (9, '2026-04-03','2026-04-28',NULL,'719',1),
       (10,'2026-05-30','2026-04-29',(SELECT UserId FROM [User] WHERE FullName= 'Степанов Михаил Артёмович'),'910',1);

INSERT INTO OrderItem(OrderId,ProductId,Quantity)
VALUES (1,(SELECT ProductId FROM [Product] WHERE Article ='А112Т4'),2),
       (2,(SELECT ProductId FROM [Product] WHERE Article ='G843H5'),1),
       (2,(SELECT ProductId FROM [Product] WHERE Article ='А112Т4'),1),
       (3,(SELECT ProductId FROM [Product] WHERE Article ='D325D4'),10),
       (4,(SELECT ProductId FROM [Product] WHERE Article ='F325D4'),5),
       (4,(SELECT ProductId FROM [Product] WHERE Article ='D325D4'),4),
       (5,(SELECT ProductId FROM [Product] WHERE Article ='G432G6'),20),
       (6,(SELECT ProductId FROM [Product] WHERE Article ='А112Т4'),2),
       (6,(SELECT ProductId FROM [Product] WHERE Article ='G843H5'),2),
       (7,(SELECT ProductId FROM [Product] WHERE Article ='C346F5'),3),
       (7,(SELECT ProductId FROM [Product] WHERE Article ='F256G6'),3),
       (8,(SELECT ProductId FROM [Product] WHERE Article ='F325D4'),1),
       (8,(SELECT ProductId FROM [Product] WHERE Article ='G432G6'),1),
       (8,(SELECT ProductId FROM [Product] WHERE Article ='H542F5'),20),
       (9,(SELECT ProductId FROM [Product] WHERE Article ='J532V5'),5),
       (9,(SELECT ProductId FROM [Product] WHERE Article ='F256G6'),1),
       (10,(SELECT ProductId FROM [Product] WHERE Article ='F256G6'),5),
       (10,(SELECT ProductId FROM [Product] WHERE Article ='J532V5'),5);
