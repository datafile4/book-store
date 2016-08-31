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

create table Stars
(
ID INT not null identity(1,1),
Name nvarchar(50) not null,

constraint PK_Stars_ID Primary Key(ID),
constraint UQ_Stars_Name unique(Name)
)

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
ImageUrl nvarchar (1000) default ('http://www.w3schools.com/w3css/img_avatar3.png') not null,
Location nvarchar(100) default('') not null,
PhoneNumber nvarchar(30)default('') not null,

constraint PK_User_ID primary key(ID),
constraint UQ_User_Username unique (Username),
constraint UQ_User_Email unique(Email),
constraint FK_Users_RoleID Foreign key (RoleID) references Roles(ID)
);

create table Ratings
(
 ID  int not null identity(1,1),
 UserID  int not null,
 GiverID int not null,
 StarID  int not null,

constraint PK_Ratings_ID primary key(ID),
constraint FK_Ratings_UserID foreign key(UserID) references Users(ID),
constraint FK_Ratings_GiverID foreign key(GiverID) references Users(ID),
constraint FK_Ratings_StarID foreign key(StarID) references Stars(ID)
)

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
Price decimal(5, 2) null,
LangID int not null,
GenreID int not null,
UserID int not null,
Confirmed   bit  DEFAULT (0) NOT NULL,
IsSold      bit    DEFAULT (0) NOT NULL,
Review nvarchar(1000) default('') not null

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
   Price, Books.ImageUrl,
   Langs.Name, Genres.Name, Review,
   Users.ID, FirstName, LastName, Username, Email, Users.ImageUrl, RoleID,  Location, PhoneNumber,
   (select COUNT(*) from Ratings where Ratings.UserID = Users.ID and Ratings.StarID = 1),
   (select COUNT(*) from Ratings where Ratings.UserID = Users.ID and Ratings.StarID = 2),
   (select COUNT(*) from Ratings where Ratings.UserID = Users.ID and Ratings.StarID = 3),
   (select COUNT(*) from Ratings where Ratings.UserID = Users.ID and Ratings.StarID = 4),
   (select COUNT(*) from Ratings where Ratings.UserID = Users.ID and Ratings.StarID = 5)
from Carts
inner join Users on Users.ID = Carts.UserID
inner join Books on Books.ID = Carts.BookID
inner join Langs on Langs.ID = Books.LangID
inner join Genres on Genres.ID = Books.GenreID
where Carts.UserID = @UserID
end
go

create procedure uspGetRandomBooks
(
@PageLength int
)
as
begin  
   select 
   Books.ID,
   Books.Name, Author,
   Price, Books.ImageUrl,
   Langs.Name, Genres.Name, Review,
   Users.ID, FirstName, LastName, Username, Email, Users.ImageUrl, RoleID,  Location, PhoneNumber,
   (select COUNT(*) from Ratings where Ratings.UserID = Users.ID and Ratings.StarID = 1),
   (select COUNT(*) from Ratings where Ratings.UserID = Users.ID and Ratings.StarID = 2),
   (select COUNT(*) from Ratings where Ratings.UserID = Users.ID and Ratings.StarID = 3),
   (select COUNT(*) from Ratings where Ratings.UserID = Users.ID and Ratings.StarID = 4),
   (select COUNT(*) from Ratings where Ratings.UserID = Users.ID and Ratings.StarID = 5)

   from Books
    inner join Users on  Books.UserID=Users.ID
	inner join Langs on Books.LangID=Langs.ID 
	inner join Genres on Books.GenreID=Genres.ID

   order by	NEWID() offset 0 Rows Fetch Next @PageLength Rows only;
end
go

create procedure uspGetBooks
(
@PageNumber int,
@PageLength int
)
as
begin  
   select 
   Books.ID,
   Books.Name, Author,
   Price, Books.ImageUrl,
   Langs.Name, Genres.Name, Review,
   Users.ID, FirstName, LastName, Username, Email, Users.ImageUrl, RoleID,  Location, PhoneNumber,
   (select COUNT(*) from Ratings where Ratings.UserID = Users.ID and Ratings.StarID = 1),
   (select COUNT(*) from Ratings where Ratings.UserID = Users.ID and Ratings.StarID = 2),
   (select COUNT(*) from Ratings where Ratings.UserID = Users.ID and Ratings.StarID = 3),
   (select COUNT(*) from Ratings where Ratings.UserID = Users.ID and Ratings.StarID = 4),
   (select COUNT(*) from Ratings where Ratings.UserID = Users.ID and Ratings.StarID = 5)

   from Books
    inner join Users on  Books.UserID=Users.ID
	inner join Langs on Books.LangID=Langs.ID 
	inner join Genres on Books.GenreID=Genres.ID

   order by	Books.ID desc offset @PageNumber * @PageLength Rows Fetch Next @PageLength Rows only;
end
go

create procedure uspGetBookInfo
(
	@BookID int
)
as 
begin 
   select
   Books.ID,
   Books.Name, Author,
   Price, Books.ImageUrl, 
   Langs.Name, Genres.Name, Review,
   Users.ID, FirstName, LastName, Username, Email, Users.ImageUrl, RoleID, Location, PhoneNumber,
   (select COUNT(*) from Ratings where Ratings.UserID = Users.ID and Ratings.StarID = 1),
   (select COUNT(*) from Ratings where Ratings.UserID = Users.ID and Ratings.StarID = 2),
   (select COUNT(*) from Ratings where Ratings.UserID = Users.ID and Ratings.StarID = 3),
   (select COUNT(*) from Ratings where Ratings.UserID = Users.ID and Ratings.StarID = 4),
   (select COUNT(*) from Ratings where Ratings.UserID = Users.ID and Ratings.StarID = 5)

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
   select 
   Books.ID,
   Books.Name, Author,
   Price, Books.ImageUrl,
   Langs.Name, Genres.Name, Review,
   Users.ID, FirstName, LastName, Username, Email, Users.ImageUrl, RoleID, Location, PhoneNumber,
   (select COUNT(*) from Ratings where Ratings.UserID = Users.ID and Ratings.StarID = 1),
   (select COUNT(*) from Ratings where Ratings.UserID = Users.ID and Ratings.StarID = 2),
   (select COUNT(*) from Ratings where Ratings.UserID = Users.ID and Ratings.StarID = 3),
   (select COUNT(*) from Ratings where Ratings.UserID = Users.ID and Ratings.StarID = 4),
   (select COUNT(*) from Ratings where Ratings.UserID = Users.ID and Ratings.StarID = 5)

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
	@price decimal(5, 2),
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

create procedure uspGetUserInfo
(
	@UserID int
)
as 
begin
	select  ID, FirstName, LastName, Username, Email, ImageUrl, RoleID,
   (select COUNT(*) from Ratings where Ratings.UserID = Users.ID and Ratings.StarID = 1),
   (select COUNT(*) from Ratings where Ratings.UserID = Users.ID and Ratings.StarID = 2),
   (select COUNT(*) from Ratings where Ratings.UserID = Users.ID and Ratings.StarID = 3),
   (select COUNT(*) from Ratings where Ratings.UserID = Users.ID and Ratings.StarID = 4),
   (select COUNT(*) from Ratings where Ratings.UserID = Users.ID and Ratings.StarID = 5)
	from Users
	where Users.ID = @UserID
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

create type StringListType
as table(item nvarchar(100))
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

create procedure uspRateUser
(
 @UserID int,
 @GiverID int,
 @StarID int
)
as
begin 
	declare @RatingID int = (select ID from Ratings where UserID = @UserID and GiverID = @GiverID)

	if @RatingID is not null
	
		if (@StarID = 0)
			delete from Ratings where ID = @RatingID
		else	
			update Ratings
				set UserID = @UserID,
					GiverID = @GiverID,
					StarID = @StarID
				where ID = @RatingID
	else
		if (@StarID != 0)
			insert into Ratings values (@UserID, @GiverID, @StarID)
end
go

create procedure uspGetRatedStar
(
 @UserID int,
 @GiverID int
)
as
begin
	select StarID from Ratings
	where UserID = @UserID and GiverID = @GiverID
end
go

create procedure uspGetFilteredBooks
(
@GenreIDs IntListType readonly,
@LangIDs IntListType readonly,
@SearchTerms StringListType readonly,
@LowPrice decimal(5, 2),
@HighPrice decimal(5, 2),
@PageNumber int,
@PageLength int
)
as
begin 
  select 
  Books.ID,
  Books.Name, Author,
  Price, Books.ImageUrl,
  Langs.Name, Genres.Name, Review,
  Users.ID, FirstName, LastName, Username, Email, Users.ImageUrl, RoleID,  Location, PhoneNumber,
   (select COUNT(*) from Ratings where Ratings.UserID = Users.ID and Ratings.StarID = 1),
   (select COUNT(*) from Ratings where Ratings.UserID = Users.ID and Ratings.StarID = 2),
   (select COUNT(*) from Ratings where Ratings.UserID = Users.ID and Ratings.StarID = 3),
   (select COUNT(*) from Ratings where Ratings.UserID = Users.ID and Ratings.StarID = 4),
   (select COUNT(*) from Ratings where Ratings.UserID = Users.ID and Ratings.StarID = 5)

  from Books
   inner join Users on  Books.UserID=Users.ID
inner join Langs on Books.LangID=Langs.ID 
inner join Genres on Books.GenreID=Genres.ID
where 
((not Exists (select 1 from @LangIDs))	or Exists (Select 1 from @LangIDs where item = Langs.ID))
and
((not Exists (select 1 from @GenreIDs)) or Exists (Select 1 from @GenreIDs where item = Genres.ID))	
and
((not Exists (select 1 from @SearchTerms)) or Exists (Select 1 from @SearchTerms where ((Books.Name like '%'+item+'%') or (Books.Author = '%'+item+'%'))))	
and
(Price between @LowPrice and @HighPrice) 

 order by Books.ID offset @PageNumber * @PageLength Rows Fetch Next @PageLength Rows only;
end
go

create procedure uspNumberOfBooksInGenre
	@ID int
as
begin
	select count(ID) from Books where Books.GenreID=@ID
end
go
create procedure uspNumberOfBooksInLang
	@ID int
as
begin
	select count(ID) from Books where Books.LangID=@ID
end
go

insert into Stars values
('1 Star'),
('2 Star'),
('3 Star'),
('4 Star'),
('5 Star')

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

exec uspRateUser 1,2,5;
exec uspRateUser 1,3,5;
exec uspRateUser 1,5,5;

insert into Books (Name, Author, ImageUrl, Price, LangID, GenreID, UserID) Values 
('A Brief History of Time', 'Stephen Hawking', 'https://upload.wikimedia.org/wikipedia/en/a/a3/BriefHistoryTime.jpg', 200.99, 4, 1 , 2 ),
('Mein Kampf', 'Adolf Hilter', 'https://images-na.ssl-images-amazon.com/images/I/41tTZSUxoyL._SX317_BO1,204,203,200_.jpg', 73.99, 2, 1 , 2 ),
('Algorithms', 'Thomas Cormen', 'https://upload.wikimedia.org/wikipedia/en/4/41/Clrs3.jpeg', 100.99, 4, 1 , 1 ),
('BrakingBad','Wince Gilligan','../images/BookImage.JPG',120.99,1,2,1),
('Harry Potter','J. K. Rowling','../images/harry_potter.jpg',180.99,1,5,2),
('Ya Malala','Ya Malala','../images/Malala.jpg',15.99,2,6,5),
('Oluler','Celil Memmedquluzade','../images/OLULER.jpg',131.99,3,2,6),
('Olasilsiizlik','Adam','http://i.idefix.com/cache/600x600-0/originals/0000000204878-1.jpg',20.99,3,3,3)