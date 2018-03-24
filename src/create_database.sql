drop database if exists TaobaoExpress;
create database TaobaoExpress;

go

use TaobaoExpress;

go

create table Products(
    Id bigint primary key identity(1, 1),
    Name varchar(100) not null,
    Image varbinary(8000) null,
    ReleaseDate datetime null,
    Created datetime not null,
    Updated datetime not null,
    ConcurrencyCheck varbinary(8) null
);

go

create table RelatedProducts(
    ProductId bigint foreign key references Products(Id),
    RelatedProductId bigint foreign key references Products(Id),
    ConcurrencyCheck varbinary(8) null,
    constraint PK_RelatedProducts primary key (ProductId, RelatedProductId),
    constraint CK_RelatedDifferent check (ProductId <> RelatedProductId)
);

go

create table ProductReviews(
    Id bigint primary key identity(1, 1),
    ProductId bigint foreign key references Products(Id) on delete cascade,
    Text varchar(1000) not null,
    UserEmail varchar(100) not null,
    Created datetime not null,
    ConcurrencyCheck varbinary(8) null
);

go

create table Retailers(
    Id bigint primary key identity(1, 1),
    Name varchar(100) not null,
    Created datetime not null,
    Updated datetime not null,
    ConcurrencyCheck varbinary(8) null
);

go

create table RetailerProducts(
    ProductId bigint foreign key references Products(Id) on delete cascade,
    RetailerId bigint foreign key references Retailers(Id) on delete cascade,
    IsManufacturer bit not null,
    constraint PK_RetailerProducts primary key (ProductId, RetailerId),
    ConcurrencyCheck varbinary(8) null
);

go

insert into Products (Name, ReleaseDate, Created, Updated) values ('Coca Cola', '2016-03-01', '2016-02-01', '2016-02-01');
insert into Products (Name, ReleaseDate, Created, Updated) values ('Pepsi', '2016-03-01', '2016-02-01', '2016-02-01');
insert into RelatedProducts (ProductId, RelatedProductId) values (1, 2);
insert into ProductReviews (ProductId, Text, UserEmail, Created) values (1, 'Awesome!', 'thomas.gassmann@hotmail.com', '2016-02-01');
insert into ProductReviews (ProductId, Text, UserEmail, Created) values (2, 'Bad!', 'thomas.gassmann@hotmail.com', '2016-03-01');
insert into Retailers (Name, Created, Updated) values ('Alibaba', '2016-03-01', '2016-02-01');
insert into Retailers (Name, Created, Updated) values ('Tencent', '2017-03-01', '2017-02-01');
insert into Retailers (Name, Created, Updated) values ('Baidu', '2016-03-01', '2017-02-01');
insert into Retailers (Name, Created, Updated) values ('Google', '2016-03-01', '2016-02-01');
insert into RetailerProducts (ProductId, RetailerId, IsManufacturer) values (1, 1, 1);
insert into RetailerProducts (ProductId, RetailerId, IsManufacturer) values (2, 1, 0);
insert into RetailerProducts (ProductId, RetailerId, IsManufacturer) values (1, 2, 0);