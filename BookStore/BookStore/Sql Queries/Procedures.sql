CREATE procedure uspAddToCart
(
 @UserID int,
 @BookID int
)
as
begin
	insert into Carts (UserID,BookID)
	values 
	(@UserID ,
    @BookID)
end

CREATE procedure uspCheckBookInCart
(
 @UserID int,
 @BookID int
)
as
begin
	select id from Carts where UserID=@UserID and BookID=@BookID
end

create procedure uspCheckEmail
(
	@Email nvarchar(100)
)
as
begin
   select id from Users where Email=lower(@Email)
end

create procedure uspCheckUsername
(
	@Username nvarchar(100)
)
as
begin
   select id from Users where Username=lower(@Username)
end

CREATE procedure uspGetCartItems
(
 @UserID int
)
as
begin	
select 
Books.Name as BookName, Author, ImageURL, Price,
Langs.Name as LangName,
Genres.Name as GenreName,
FirstName, LastName, Username, Email

from Books 
inner join Langs on Langs.ID = Books.LangID
inner join Genres on Genres.ID = Books.GenreID
inner join Users on Users.ID = Books.UserID 
where UserID = @UserID

end


create procedure uspUploadBook
(
	@name nvarchar(100),
	@author nvarchar(100),
	@ImageURL nvarchar(100),
	@price decimal,
	@langID int, 
	@genreID int,
	@userID int
)
as 
begin 
	insert into Books (name, author, ImageURL, price, langID, genreID, userID)
	 values(@name, @author, @ImageURL, @price, @langID, @genreID, @userID)
end


create procedure uspGetUserInfoFromID
(
	@UserID int
)
as 
begin
	select email, lastname, firstname, username, roleID from Users where ID=@UserID
end

create procedure uspGetUserInfo
(
	@Username nvarchar(100)
)
as 
begin
	select email, lastname, firstname, id, roleID from Users where ID=@UserID
end

create procedure uspRemoveFromCart
(
	@UserID int,
	@BookID int
)
as
begin 
 delete from Carts 
 where BookID=@BookID 
 and UserID=@UserID 
end

CREATE procedure uspLogin
(
	@Username nvarchar(100), 
	@Password nvarchar(100)
)
As
begin
	select ID from Users where Username=lower(@Username) and Password=@Password 
end

create procedure uspInsertIntoUserLogins
(
  @UserID int, 
  @GuidStr nvarchar(100),
  @expireDate nvarchar(100),
  @now  nvarchar(100)
)
as 
begin
	
	insert into UserLogins (UserID,GUID,Expires,LastLogin)
    values (@UserID,@GuidStr,@expireDate,@now)

end


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
 insert into Users
  (FirstName, LastName, UserName, Password, Email) 
 values 
 (@FirstName, @LastName, lower(@UserName), @Password, lower(@Email))
end


create procedure uspUpdateLastLogin
(
	@Guid nvarchar(36), 
	@LastLogin datetimeoffset(7)
)
as
begin
	update UserLogins set LastLogin=@LastLogin where GUID=@Guid
end

create procedure uspGetUserID
(
 @Guid nvarchar(36)
)

as
begin 
	 select UserID from UserLogins where GUID=@Guid
end

CREATE procedure uspGetAllBooks
as 
begin 
   select Books.Name, Author,
   ImageURL, Price,
   Langs.Name as Language,
   Genres.Name as Genre,
   FirstName, LastName,
   Email, Username, Book.ID
   from Books
    inner join Users on  Books.UserID=Users.ID
	inner join Langs on Books.LangID=Langs.ID 
	inner join Genres on Books.GenreID=Genres.ID
end

create procedure uspConfirmBook
(
  @bookID int
)
as
begin
	Update Books Set Books.State='True' where Books.ID=@bookID
end

create procedure uspGetBookInfo
(
	@BookID int
)
as 
begin 
   select Books.Name, Author,
   ImageURL, Price,
   Langs.Name as Language,
   Genres.Name as Genre,
   FirstName, LastName,
   Email, Username 
   from Books
    inner join Users on  Books.UserID=Users.ID
	inner join Langs on Books.LangID=Langs.ID 
	inner join Genres on Books.GenreID=Genres.ID
	where Books.ID = @BookID
end

create procedure uspGetAllGenres
as 
begin
	select ID ,Name 
	from Genres
end

create procedure uspGetAllLanguages
as
begin 
	select ID, Name
	 from Langs
end


CREATE procedure uspGetUserRole
(
 @Guid nvarchar(36)
)
as
begin 
	 select Users.RoleID 
	 from Users
	 inner join UserLogins
     on Users.ID = UserLogins.UserID 
     where GUID = @Guid
end


CREATE procedure uspSetRole
(
 @Role int,
 @UserID int
)
as
begin
	update Users 
	set Users.RoleID=@Role 
	where Users.ID=@UserID
end