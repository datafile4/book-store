create database BookStore
use BookStore
go

Create Table Books(

ID INT NOT NULL identity(1,1),
Name nvarchar(50) Null,
Author nvarchar(50) null,
ImageURL nvarchar(1000) null,
Price decimal null,
LangID int not null,
GenreID int not null,
UserID int not null,
State       BIT             DEFAULT (0) NOT NULL,
StateOfSelling BIT             DEFAULT (0) NOT NULL,

constraint PK_Books_ID Primary Key (ID),
constraint FK_Books_LangID Foreign key (LangID) references Langs(ID),
constraint FK_Books_GenreID Foreign key (GenreID) references Genres(ID),
constraint FK_Books_UserID Foreign key (UserID) references Users(ID)
);

create table Users(
ID int not null identity(1,1),
FirstName nvarchar(100) not null,
LastName nvarchar(100) not null,
Username nvarchar(100) not null,
Password nvarchar(100) not null,
Email nvarchar(100) not null,
RoleID int default (1) not null,

constraint PK_User_ID primary key(ID),
constraint UQ_User_Username unique (Username),
constraint UQ_User_Email unique(Email),
constraint FK_Users_RoleID Foreign key (RoleID) references Roles(ID)
);

Create Table Langs(
ID INT not null identity(1,1),
Name nvarchar(50) not null,

constraint PK_Langs_ID Primary Key(ID),
constraint UQ_Langs_Name unique(Name)
);

Create Table Genres(
ID int not null identity(1,1),
Name nvarchar(50) null,

constraint PK_Genre_ID primary key(ID),
constraint UQ_Genre_Name unique(Name)
);

Create Table Carts(
ID int not null identity(1,1),
UserID int not null,
BookID int not null,

constraint PK_Carts_ID primary key(ID),
constraint FK_Carts_UserID Foreign key (UserID) references Users(ID),
constraint FK_Carts_BookID Foreign key (BookID) references Books(ID)
);

Create Table UserLogins(
ID int not null identity(1,1),
UserID int not null,
GUID nvarchar(36) null,
Expires   DATETIMEOFFSET not NULL, 
LastLogin DATETIMEOFFSET not NULL, 

constraint PK_Tokens_ID primary key(ID),
constraint FK_Tokens_UserID Foreign key (UserID) references Users(ID)
);

create table Roles
(
ID int Not NUll identity(1,1), 
Name nvarchar(200) Not Null,

constraint PK_Roles_ID primary key (ID),
constraint UQ_Roles_Name unique(Name)
);