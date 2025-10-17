-- ===============================
-- CMS-Full schema for MyBlogDB
-- ===============================
-- Thay đổi DB NAME nếu bạn muốn
IF DB_ID('MyBlogDB') IS NULL
BEGIN
    CREATE DATABASE MyBlogDB;
END
GO

USE MyBlogDB;
GO

-- -----------------------
-- 1) Admins
-- -----------------------
IF OBJECT_ID('dbo.Admins','U') IS NULL
BEGIN
CREATE TABLE dbo.Admins (
    AdminId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(200) NULL,
    Phone NVARCHAR(50) NULL,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    Role NVARCHAR(50) NOT NULL DEFAULT 'superadmin',  -- ví dụ: superadmin, admin, moderator
    Status NVARCHAR(20) NOT NULL DEFAULT 'active',    -- active | blocked
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);
END
GO

-- -----------------------
-- 2) Users
-- -----------------------
IF OBJECT_ID('dbo.Users','U') IS NULL
BEGIN
CREATE TABLE dbo.Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(200) NULL,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    Phone NVARCHAR(50) NULL,
    Role NVARCHAR(50) NOT NULL DEFAULT 'member',     -- member | guest | other
    PasswordHash NVARCHAR(255) NOT NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'active',   -- active | blocked
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);
END
GO

-- -----------------------
-- 3) Categories
-- -----------------------
IF OBJECT_ID('dbo.Categories','U') IS NULL
BEGIN
CREATE TABLE dbo.Categories (
    CategoryId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL UNIQUE,
    Description NVARCHAR(1000) NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'active',
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);
END
GO

-- -----------------------
-- 4) Tags
-- -----------------------
IF OBJECT_ID('dbo.Tags','U') IS NULL
BEGIN
CREATE TABLE dbo.Tags (
    TagId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL UNIQUE,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);
END
GO

-- -----------------------
-- 5) Videos
-- -----------------------
IF OBJECT_ID('dbo.Videos','U') IS NULL
BEGIN
CREATE TABLE dbo.Videos (
    VideoId INT IDENTITY(1,1) PRIMARY KEY,
    VideoCode NVARCHAR(200) NULL,   -- optional custom code/id
    Title NVARCHAR(400) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    CoverImage NVARCHAR(1024) NULL, -- URL or path
    Content NVARCHAR(MAX) NULL,     -- markdown or HTML
    CreatedByAdminId INT NULL,
    Status NVARCHAR(30) NOT NULL DEFAULT 'published', -- published | draft | archived
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_Videos_Admins FOREIGN KEY (CreatedByAdminId) REFERENCES dbo.Admins(AdminId) ON DELETE SET NULL
);
CREATE INDEX IX_Videos_Title ON dbo.Videos(Title);
END
GO

-- -----------------------
-- 6) Posts
-- -----------------------
IF OBJECT_ID('dbo.Posts','U') IS NULL
BEGIN
CREATE TABLE dbo.Posts (
    PostId INT IDENTITY(1,1) PRIMARY KEY,
    PostCode NVARCHAR(200) NULL,
    Title NVARCHAR(400) NOT NULL,
    Slug NVARCHAR(400) NULL UNIQUE,
    Description NVARCHAR(MAX) NULL,
    CoverImage NVARCHAR(1024) NULL,
    Content NVARCHAR(MAX) NULL,     -- markdown or HTML
    CreatedByAdminId INT NULL,
    Status NVARCHAR(30) NOT NULL DEFAULT 'published', -- published | draft | archived
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_Posts_Admins FOREIGN KEY (CreatedByAdminId) REFERENCES dbo.Admins(AdminId) ON DELETE SET NULL
);
CREATE INDEX IX_Posts_Title ON dbo.Posts(Title);
CREATE INDEX IX_Posts_Slug ON dbo.Posts(Slug);
END
GO

-- -----------------------
-- 7) PostCategories (many-to-many)
-- -----------------------
IF OBJECT_ID('dbo.PostCategories','U') IS NULL
BEGIN
CREATE TABLE dbo.PostCategories (
    PostCategoryId INT IDENTITY(1,1) PRIMARY KEY,
    PostId INT NOT NULL,
    CategoryId INT NOT NULL,
    CONSTRAINT FK_PostCategories_Posts FOREIGN KEY (PostId) REFERENCES dbo.Posts(PostId) ON DELETE CASCADE,
    CONSTRAINT FK_PostCategories_Categories FOREIGN KEY (CategoryId) REFERENCES dbo.Categories(CategoryId) ON DELETE CASCADE,
    CONSTRAINT UQ_PostCategory UNIQUE (PostId, CategoryId)
);
CREATE INDEX IX_PostCategories_PostId ON dbo.PostCategories(PostId);
CREATE INDEX IX_PostCategories_CategoryId ON dbo.PostCategories(CategoryId);
END
GO

-- -----------------------
-- 8) VideoCategories (many-to-many)
-- -----------------------
IF OBJECT_ID('dbo.VideoCategories','U') IS NULL
BEGIN
CREATE TABLE dbo.VideoCategories (
    VideoCategoryId INT IDENTITY(1,1) PRIMARY KEY,
    VideoId INT NOT NULL,
    CategoryId INT NOT NULL,
    CONSTRAINT FK_VideoCategories_Videos FOREIGN KEY (VideoId) REFERENCES dbo.Videos(VideoId) ON DELETE CASCADE,
    CONSTRAINT FK_VideoCategories_Categories FOREIGN KEY (CategoryId) REFERENCES dbo.Categories(CategoryId) ON DELETE CASCADE,
    CONSTRAINT UQ_VideoCategory UNIQUE (VideoId, CategoryId)
);
CREATE INDEX IX_VideoCategories_VideoId ON dbo.VideoCategories(VideoId);
CREATE INDEX IX_VideoCategories_CategoryId ON dbo.VideoCategories(CategoryId);
END
GO

-- -----------------------
-- 9) PostTags
-- -----------------------
IF OBJECT_ID('dbo.PostTags','U') IS NULL
BEGIN
CREATE TABLE dbo.PostTags (
    PostTagId INT IDENTITY(1,1) PRIMARY KEY,
    PostId INT NOT NULL,
    TagId INT NOT NULL,
    CONSTRAINT FK_PostTags_Posts FOREIGN KEY (PostId) REFERENCES dbo.Posts(PostId) ON DELETE CASCADE,
    CONSTRAINT FK_PostTags_Tags FOREIGN KEY (TagId) REFERENCES dbo.Tags(TagId) ON DELETE CASCADE,
    CONSTRAINT UQ_PostTag UNIQUE (PostId, TagId)
);
CREATE INDEX IX_PostTags_PostId ON dbo.PostTags(PostId);
CREATE INDEX IX_PostTags_TagId ON dbo.PostTags(TagId);
END
GO

-- -----------------------
-- 10) VideoTags
-- -----------------------
IF OBJECT_ID('dbo.VideoTags','U') IS NULL
BEGIN
CREATE TABLE dbo.VideoTags (
    VideoTagId INT IDENTITY(1,1) PRIMARY KEY,
    VideoId INT NOT NULL,
    TagId INT NOT NULL,
    CONSTRAINT FK_VideoTags_Videos FOREIGN KEY (VideoId) REFERENCES dbo.Videos(VideoId) ON DELETE CASCADE,
    CONSTRAINT FK_VideoTags_Tags FOREIGN KEY (TagId) REFERENCES dbo.Tags(TagId) ON DELETE CASCADE,
    CONSTRAINT UQ_VideoTag UNIQUE (VideoId, TagId)
);
CREATE INDEX IX_VideoTags_VideoId ON dbo.VideoTags(VideoId);
CREATE INDEX IX_VideoTags_TagId ON dbo.VideoTags(TagId);
END
GO

-- -----------------------
-- 11) PostMeta (key-value)
-- -----------------------
IF OBJECT_ID('dbo.PostMeta','U') IS NULL
BEGIN
CREATE TABLE dbo.PostMeta (
    PostMetaId INT IDENTITY(1,1) PRIMARY KEY,
    PostId INT NOT NULL,
    MetaKey NVARCHAR(200) NOT NULL,
    MetaValue NVARCHAR(MAX) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_PostMeta_Posts FOREIGN KEY (PostId) REFERENCES dbo.Posts(PostId) ON DELETE CASCADE,
    CONSTRAINT UQ_PostMeta_Key UNIQUE (PostId, MetaKey)
);
CREATE INDEX IX_PostMeta_PostId ON dbo.PostMeta(PostId);
END
GO

-- -----------------------
-- 12) VideoMeta (key-value)
-- -----------------------
IF OBJECT_ID('dbo.VideoMeta','U') IS NULL
BEGIN
CREATE TABLE dbo.VideoMeta (
    VideoMetaId INT IDENTITY(1,1) PRIMARY KEY,
    VideoId INT NOT NULL,
    MetaKey NVARCHAR(200) NOT NULL,
    MetaValue NVARCHAR(MAX) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_VideoMeta_Videos FOREIGN KEY (VideoId) REFERENCES dbo.Videos(VideoId) ON DELETE CASCADE,
    CONSTRAINT UQ_VideoMeta_Key UNIQUE (VideoId, MetaKey)
);
CREATE INDEX IX_VideoMeta_VideoId ON dbo.VideoMeta(VideoId);
END
GO

-- -----------------------
-- 13) PostComments
-- -----------------------
IF OBJECT_ID('dbo.PostComments','U') IS NULL
BEGIN
CREATE TABLE dbo.PostComments (
    CommentId INT IDENTITY(1,1) PRIMARY KEY,
    PostId INT NOT NULL,
    UserId INT NULL,
    Content NVARCHAR(MAX) NOT NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'visible', -- visible | hidden | removed
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_PostComments_Posts FOREIGN KEY (PostId) REFERENCES dbo.Posts(PostId) ON DELETE CASCADE,
    CONSTRAINT FK_PostComments_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId) ON DELETE SET NULL
);
CREATE INDEX IX_PostComments_PostId ON dbo.PostComments(PostId);
CREATE INDEX IX_PostComments_UserId ON dbo.PostComments(UserId);
END
GO

-- -----------------------
-- 14) VideoComments
-- -----------------------
IF OBJECT_ID('dbo.VideoComments','U') IS NULL
BEGIN
CREATE TABLE dbo.VideoComments (
    CommentId INT IDENTITY(1,1) PRIMARY KEY,
    VideoId INT NOT NULL,
    UserId INT NULL,
    Content NVARCHAR(MAX) NOT NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'visible',
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_VideoComments_Videos FOREIGN KEY (VideoId) REFERENCES dbo.Videos(VideoId) ON DELETE CASCADE,
    CONSTRAINT FK_VideoComments_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId) ON DELETE SET NULL
);
CREATE INDEX IX_VideoComments_VideoId ON dbo.VideoComments(VideoId);
CREATE INDEX IX_VideoComments_UserId ON dbo.VideoComments(UserId);
END
GO

-- -----------------------
-- 15) Triggers to update UpdatedAt on update (for tables with UpdatedAt)
-- -----------------------
-- Helper to create trigger text safely: create for each table

-- Admins
IF OBJECT_ID('dbo.trg_Admins_Update','TR') IS NULL
EXEC('
CREATE TRIGGER dbo.trg_Admins_Update
ON dbo.Admins
AFTER UPDATE
AS
BEGIN
  SET NOCOUNT ON;
  UPDATE A
  SET UpdatedAt = SYSUTCDATETIME()
  FROM dbo.Admins A
  INNER JOIN inserted i ON A.AdminId = i.AdminId;
END
');
GO

-- Users
IF OBJECT_ID('dbo.trg_Users_Update','TR') IS NULL
EXEC('
CREATE TRIGGER dbo.trg_Users_Update
ON dbo.Users
AFTER UPDATE
AS
BEGIN
  SET NOCOUNT ON;
  UPDATE U
  SET UpdatedAt = SYSUTCDATETIME()
  FROM dbo.Users U
  INNER JOIN inserted i ON U.UserId = i.UserId;
END
');
GO

-- Categories
IF OBJECT_ID('dbo.trg_Categories_Update','TR') IS NULL
EXEC('
CREATE TRIGGER dbo.trg_Categories_Update
ON dbo.Categories
AFTER UPDATE
AS
BEGIN
  SET NOCOUNT ON;
  UPDATE C
  SET UpdatedAt = SYSUTCDATETIME()
  FROM dbo.Categories C
  INNER JOIN inserted i ON C.CategoryId = i.CategoryId;
END
');
GO

-- Videos
IF OBJECT_ID('dbo.trg_Videos_Update','TR') IS NULL
EXEC('
CREATE TRIGGER dbo.trg_Videos_Update
ON dbo.Videos
AFTER UPDATE
AS
BEGIN
  SET NOCOUNT ON;
  UPDATE V
  SET UpdatedAt = SYSUTCDATETIME()
  FROM dbo.Videos V
  INNER JOIN inserted i ON V.VideoId = i.VideoId;
END
');
GO

-- Posts
IF OBJECT_ID('dbo.trg_Posts_Update','TR') IS NULL
EXEC('
CREATE TRIGGER dbo.trg_Posts_Update
ON dbo.Posts
AFTER UPDATE
AS
BEGIN
  SET NOCOUNT ON;
  UPDATE P
  SET UpdatedAt = SYSUTCDATETIME()
  FROM dbo.Posts P
  INNER JOIN inserted i ON P.PostId = i.PostId;
END
');
GO

-- PostComments
IF OBJECT_ID('dbo.trg_PostComments_Update','TR') IS NULL
EXEC('
CREATE TRIGGER dbo.trg_PostComments_Update
ON dbo.PostComments
AFTER UPDATE
AS
BEGIN
  SET NOCOUNT ON;
  UPDATE C
  SET UpdatedAt = SYSUTCDATETIME()
  FROM dbo.PostComments C
  INNER JOIN inserted i ON C.CommentId = i.CommentId;
END
');
GO

-- VideoComments
IF OBJECT_ID('dbo.trg_VideoComments_Update','TR') IS NULL
EXEC('
CREATE TRIGGER dbo.trg_VideoComments_Update
ON dbo.VideoComments
AFTER UPDATE
AS
BEGIN
  SET NOCOUNT ON;
  UPDATE C
  SET UpdatedAt = SYSUTCDATETIME()
  FROM dbo.VideoComments C
  INNER JOIN inserted i ON C.CommentId = i.CommentId;
END
');
GO

-- -----------------------
-- 16) Optional helper views
-- -----------------------
IF OBJECT_ID('dbo.vw_PostsWithAdmin','V') IS NULL
EXEC('
CREATE VIEW dbo.vw_PostsWithAdmin
AS
SELECT p.PostId, p.Title, p.Slug, p.Status, p.CreatedAt, p.UpdatedAt,
       a.AdminId, a.Email AS AdminEmail
FROM dbo.Posts p
LEFT JOIN dbo.Admins a ON p.CreatedByAdminId = a.AdminId;
');
GO

IF OBJECT_ID('dbo.vw_VideosWithAdmin','V') IS NULL
EXEC('
CREATE VIEW dbo.vw_VideosWithAdmin
AS
SELECT v.VideoId, v.Title, v.VideoCode, v.Status, v.CreatedAt, v.UpdatedAt,
       a.AdminId, a.Email AS AdminEmail
FROM dbo.Videos v
LEFT JOIN dbo.Admins a ON v.CreatedByAdminId = a.AdminId;
');
GO

PRINT 'Schema created/ensured successfully';
GO
