use master
if exists (select 1 from sys.databases where name = 'BookStore')
begin
	alter database BookStore set single_user with rollback immediate;
	drop database  BookStore;
end
create database BookStore     

go  
use BookStore
go



Create Table Langs(
ID INT not null identity(1,1),
Name nvarchar(50) not null,

constraint PK_Langs_ID Primary Key(ID),
constraint UQ_Langs_Name unique(Name)
);

Create Table Genres(
ID int not null identity(1,1),
Name nvarchar(50) not null,

constraint PK_Genre_ID primary key(ID),
constraint UQ_Genre_Name unique(Name)
);

create table Roles
(
ID int Not NUll identity(1,1), 
Name nvarchar(200) Not Null,

constraint PK_Roles_ID primary key (ID),
constraint UQ_Roles_Name unique(Name)
);
create table Users(
ID int not null identity(1,1),
FirstName nvarchar(100) not null,
LastName nvarchar(100) not null,
Username nvarchar(100) not null,
Password nvarchar(100) not null,
Email nvarchar(100) not null,
RoleID int default (1) not null,
ImageUrl nvarchar (1000) default ('images/Jackie-Chan.jpg') not null,

constraint PK_User_ID primary key(ID),
constraint UQ_User_Username unique (Username),
constraint UQ_User_Email unique(Email),
constraint FK_Users_RoleID Foreign key (RoleID) references Roles(ID)
);

Create Table UserLogins(
ID int not null identity(1,1),
UserID int not null,
GUID nvarchar(36) null,
Expires   datetimeoffset(7) not NULL, 
LastLogin datetimeoffset(7) not NULL, 

constraint PK_UserLogins_ID primary key(ID),
constraint FK_UserLogins_UserID Foreign key (UserID) references Users(ID),
constraint UQ_UserLogins_GUID unique (GUID)
);


Create Table Books(

ID INT NOT NULL identity(1,1),
Name nvarchar(50) Null,
Author nvarchar(50) null,
ImageUrl nvarchar(1000) null,
Price decimal null,
LangID int not null,
GenreID int not null,
UserID int not null,
Confirmed   bit  DEFAULT (0) NOT NULL,
IsSold      bit    DEFAULT (0) NOT NULL,

constraint PK_Books_ID Primary Key (ID),
constraint FK_Books_LangID Foreign key (LangID) references Langs(ID),
constraint FK_Books_GenreID Foreign key (GenreID) references Genres(ID),
constraint FK_Books_UserID Foreign key (UserID) references Users(ID)
);


Create Table Carts(
ID int not null identity(1,1),
UserID int not null,
BookID int not null,

constraint PK_Carts_ID primary key(ID),
constraint FK_Carts_UserID Foreign key (UserID) references Users(ID),
constraint FK_Carts_BookID Foreign key (BookID) references Books(ID)
);
go

CREATE procedure uspAddToCart
(
 @UserID int,
 @BookID int
)
as
begin
	insert into Carts (UserID, BookID)
	values (@UserID, @BookID)
end
go

CREATE procedure uspCheckBookInCart
(
 @UserID int,
 @BookID int
)
as
begin
	select id from Carts 
	where UserID = @UserID and BookID = @BookID
end
go

create procedure uspCheckEmail
(
	@Email nvarchar(100)
)
as
begin
   select id from Users where Email = @Email
end
go

create procedure uspCheckUsername
(
	@Username nvarchar(100)
)
as
begin
   select id from Users 
   where Username = @Username
end
go

CREATE procedure uspGetCartItems
(
 @UserID int
)
as
begin	
select 
   Books.ID,
   Books.Name, Author,
   Books.ImageUrl, Price,
   Langs.Name, Genres.Name,
   Users.ID, FirstName, LastName, Username, Email, Users.ImageUrl, RoleID
from Carts
inner join Users on Users.ID = Carts.UserID
inner join Books on Books.ID = Carts.BookID
inner join Langs on Langs.ID = Books.LangID
inner join Genres on Genres.ID = Books.GenreID
where Carts.UserID = @UserID
end
go

create procedure uspGetAllBooks
as
begin 
   select Books.ID,
   Books.Name, Author,
   Books.ImageUrl, Price,
   Langs.Name, Genres.Name,
   Users.ID, FirstName, LastName, Username, Email, Users.ImageUrl, RoleID

   from Books
    inner join Users on  Books.UserID=Users.ID
	inner join Langs on Books.LangID=Langs.ID 
	inner join Genres on Books.GenreID=Genres.ID
end
go

create procedure uspGetBookInfo
(
	@BookID int
)
as 
begin 
   select
   Books.Name, Author,
   Books.ImageUrl, Price,
   Langs.Name, Genres.Name,
   Users.ID, FirstName, LastName, Username, Email, Users.ImageUrl, RoleID

   from Books
    inner join Users on  Books.UserID=Users.ID
	inner join Langs on Books.LangID=Langs.ID 
	inner join Genres on Books.GenreID=Genres.ID
	where Books.ID = @BookID
end
go

create procedure uspGetUnconfirmedBooks
as
begin
select Books.ID,
   Books.Name, Author,
   Books.ImageUrl, Price,
   Langs.Name, Genres.Name,
   Users.ID, FirstName, LastName, Username, Email, Users.ImageUrl, RoleID

   from Books
    inner join Users on  Books.UserID=Users.ID
	inner join Langs on Books.LangID=Langs.ID 
	inner join Genres on Books.GenreID=Genres.ID

where Books.Confirmed = 'false';
end
go

create procedure uspUploadBook
(
	@name nvarchar(100),
	@author nvarchar(100),
	@ImageUrl nvarchar(100),
	@price decimal,
	@langID int, 
	@genreID int,
	@userID int
)
as 
begin 
	insert into Books (name, author, ImageUrl, price, langID, genreID, userID)
	 values(@name, @author, @ImageUrl, @price, @langID, @genreID, @userID)
end
go

create procedure uspGetUserInfoFromID
(
	@UserID int
)
as 
begin
	select  email, lastname, firstname, username, roleID from Users
	where ID = @UserID
end
go

create procedure uspGetUserInfo
(
	@Username nvarchar(100)
)
as 
begin
	select id, email, lastname, firstname, roleID from Users
	where username = @Username
end
go

create procedure uspRemoveFromCart
(
	@UserID int,
	@BookID int
)
as
begin 
 delete from Carts 
 where 
 BookID = @BookID 
 and UserID = @UserID 
end
go

CREATE procedure uspLogin
(
	@Username nvarchar(100), 
	@Password nvarchar(100)
)
As
begin
	select ID from Users 
	where 
	(Username = @Username or Email = @Username) 
	and Password = @Password 
end
go

create procedure uspInsertUserLogin
(
  @UserID int, 
  @GuidStr nvarchar(100),
  @expireDate nvarchar(100),
  @now  datetimeoffset(7)
)
as 
begin
	insert into UserLogins (UserID, GUID, Expires, LastLogin)
    values (@UserID, @GuidStr, @expireDate, @now)
end
go

CREATE procedure uspRegister
(
	@FirstName nvarchar(100), 
	@LastName nvarchar(100),
	@UserName nvarchar(100),
	@Password nvarchar(100),
	@Email nvarchar (100) 
)
as
Begin
 insert into Users (FirstName, LastName, UserName, Password, Email) 
 values 
 (@FirstName, @LastName, @UserName, @Password, @Email)
end
go

create procedure uspUpdateLastLogin
(
	@Guid nvarchar(36), 
	@LastLogin datetimeoffset(7)
)
as
begin
	update UserLogins 
	set LastLogin = @LastLogin 
	where GUID = @Guid
end
go

create procedure uspGetUserIDFromGuid
(
 @Guid nvarchar(36)
)
as
begin 
	 select UserID from UserLogins 
	 where GUID = @Guid
end
go

create procedure uspConfirmBook
(
  @BookID int
)
as
begin
	update Books
	set Confirmed = 'True' 
	where ID = @BookID
end
go

create type IntListType
as table(item int)
go

create procedure uspConfirmBooks
(
  @BookIDs IntListType readonly
)
as
begin
	update Books
	set Confirmed = 'True' 
	where Exists 
	(select 1 from @BookIDs where item = Books.ID)
end
go

create procedure uspDeleteBook
(
@BookID int
)
as 
begin 
	Delete From Books  
	where ID = @BookID
end
go

create procedure uspDeleteBooks
(
@BookIDs IntListType readonly
)
as
begin
	Delete From Books
	where Exists 
	(select 1 from @BookIDs where item = Books.ID)
end
go

create procedure uspGetAllGenres
as 
begin
	select ID, Name 
	from Genres
end
go

create procedure uspGetAllLanguages
as
begin 
	select ID, Name
	from Langs
end
go

CREATE procedure uspGetAllRoles
as
begin
	select ID, Name
    from Roles
end
go

create procedure uspGetUserRole
(
 @Guid nvarchar(36)
)
as
begin 
	 select RoleID from Users
	 inner join UserLogins
     on Users.ID = UserLogins.UserID 
     where GUID = @Guid
end
go

CREATE procedure uspSetRole
(
 @Username nvarchar(100),
 @Role int
)
as
begin
	update Users 
	set RoleID = @Role 
	where Username = @Username
end
go


create procedure uspGetUserID
(
 @Guid nvarchar(36)
)
as
begin 
	 select UserID from UserLogins 
	 where GUID=@Guid
end
go

insert into Roles values
('User'),
('Moderator'),
('Admin')

insert into Users  (FirstName, LastName, Username, Password, Email, RoleID) values
('Amiraslan', 'Bakhshili', 'emiraslan', 'emiraslan24', 'aslanbaxisli.ba@gmail.com', 3),
('Ali', 'Kerimli', 'alikerim', 'alikerim2', 'alikerimli94@gmail.com', 3),
('Orxan', 'Alikhanov', 'orxan', 'orxan123', 'orxanalixanov@gmail.com', 3),
('Nadir', 'Hasimov', 'nadir', 'nadir123', 'nadirhasimov@gmail.com', 1),
('Emil', 'Alasgarov', 'emil', 'emil123', 'emilalasgarov@gmail.com', 3),
('Ali', 'Kerimli', 'alike', '1234567', 'ali@gmai.com', 3)

insert into Genres values
('Biography'),
('Crime'),
('Detective'),
('Fantastic'),
('Romance'),
('Sci-Fi')

insert into Langs values
('Azerbaijan'),
('English'),
('Russian'),
('Spanish'),
('Turkish')

insert into Books (Name, Author, ImageUrl, Price, LangID, GenreID, UserID) Values 
('BrakingBad','Wince Gilligan','../images/BookImage.JPG',120,1,2,1),
('Harry Potter','J. K. Rowling','../images/harry_potter.jpg',180,1,5,2),
('Ya Malala','Ya Malala','../images/Malala.jpg',15,2,6,5),
('Oluler','Celil Memmedquluzade','../images/OLULER.jpg',131,3,2,6),
('Olasilsiizlik','Adam','http://i.idefix.com/cache/600x600-0/originals/0000000204878-1.jpg',20,3,3,3)