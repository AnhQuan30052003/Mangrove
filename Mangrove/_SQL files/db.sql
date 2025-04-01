use mangrove

--- [Drop tables] --
drop table if exists tblHome
drop table if exists tblSetting

drop table if exists tblStage
drop table if exists tblIndividual
drop table if exists tblMangrove

drop table if exists tblPhotos
drop table if exists tblDistributiton
drop table if exists tblAdmin

-- [Add tables] --
-- Bảng cho admin
create table tblAdmin
(
    _id varchar(36) primary key,
    _email varchar(256) not null,
    _username nvarchar(50) not null,
    _password varchar(50) not null,
    _codeSendEmail varchar(20) not null
)

-- Bảng cho trang setting
create table tblSetting
(
	_id varchar(36) primary key,
     _logoImg nvarchar(50) not null,
     _footerBgImg nvarchar(50) not null,
     _phone varchar(20) not null,
     _email varchar(256) not null,

     _schoolNameVI nvarchar(256) not null,
     _schoolNameEN nvarchar(256) not null,

     _facultyVI nvarchar(256) not null,
     _facultyEN nvarchar(256) not null,

     _descriptionWebsiteVI nvarchar(max) not null,
     _descriptionWebsiteEN nvarchar(max) not null,

     _addressVI nvarchar(256) not null,
     _addressEN nvarchar(256) not null
)

-- Bảng cho trang home
 create table tblHome
 (
	
	_id varchar(36) primary key,
     _bannerImg nvarchar(50) not null,
     _itemRecent int not null,

     _bannerTitleVI nvarchar(256) not null,
     _bannerTitleEN nvarchar(256) not null,

     _purposeVI nvarchar(max) not null,
     _purposeEN nvarchar(max) not null
 )

-- Bảng tổng quan cây ngập mặn
create table tblMangrove
(
    _id varchar(36) not null primary key,

    _nameVI nvarchar(256) not null,
    _nameEN nvarchar(256) not null,

    _commonNameVI nvarchar(256) not null,
    _commonNameEN nvarchar(256) not null,

    _scientificName nvarchar(256) not null,
    _familia nvarchar(256) not null,
    _mainImage nvarchar(max) not null,

    _morphologyVI nvarchar(max) not null,
    _morphologyEN nvarchar(max) not null,

    _ecologyVI nvarchar(max) not null,
    _ecologyEN nvarchar(max) not null,

    _distributionVI nvarchar(256) not null,
    _distributionEN nvarchar(256) not null,

    _conservationStatusVI nvarchar(256) not null,
    _conservationStatusEN nvarchar(256) not null,

    _useVI nvarchar(max) not null,
    _useEN nvarchar(max) not null,

    _view bigint not null,
    _updateLast datetime not null
)

-- Bảng cá thể
create table tblIndividual
(
    _id varchar(36) not null primary key,
    _idMangrove varchar(36) foreign key references tblMangrove(_id),
    
    _longitude varchar(256) null,
    _latitude varchar(256) null,

    _positionVI nvarchar(256) not null,
    _positionEN nvarchar(256) not null,

    _updateLast datetime not null,
    _qrName nvarchar(256) not null,
    _view bigint not null
)

-- Bảng giai đoạn
create table tblStage
(
    _id varchar(36) not null primary key,
    _idIndividual varchar(36) foreign key references tblIndividual(_id),
    _mainImage nvarchar(256) not null,
    _surveyDay datetime not null,

    _nameVI nvarchar(256) not null,
    _nameEN nvarchar(256) not null,

    _weatherVI nvarchar(256) null,
    _weatherEN nvarchar(256) null
)

-- Bảng lưu trữ ảnh
create table tblPhotos
(
    _id varchar(36) not null primary key,
    _idObj varchar(36) not null,
    _imageName nvarchar(256) not null,

    _noteImgVI nvarchar(256) null,
    _noteImgEN nvarchar(256) null
)

-- Bảng phân bố - bản đồ
create table tblDistributiton (
    _id varchar(36) not null primary key,
    _imageMap nvarchar(256) not null,
    
    _mapNameVI nvarchar(256) not null,
    _mapNameEN nvarchar(256) not null
)