create database mangrove
use mangrove

use master
drop database mangrove

--- [Drop tables] --
drop table if exists tblHome

drop table if exists tblStage
drop table if exists tblIndividual
drop table if exists tblMangrove

drop table if exists tblPhotos


-- [Add tables] --
-- Bảng cho trang home
 create table tblHome
 (
     _logoImg nvarchar(256) not null,
     _bannerImg nvarchar(256) not null,
     _footerImg nvarchar(256) not null,

     _bannerTitle nvarchar(256) not null,
     _bannerTitleVI nvarchar(256) not null,
     _bannerTitleEN nvarchar(256) not null,

     _purpose nvarchar(256) not null,
     _purposeVI nvarchar(256) not null,
     _purposeEN nvarchar(256) not null,

     _itemRecent int not null,

     _descriptionWebsite nvarchar(256) not null,
     _descriptionWebsiteVI nvarchar(256) not null,
     _descriptionWebsiteEN nvarchar(256) not null,

     _address nvarchar(256) not null,
     _addressVI nvarchar(256) not null,
     _addressEN nvarchar(256) not null,

     _phone varchar(15) not null,
     _email varchar(256) not null,
 )

-- Bảng tổng quan cây ngập mặn
create table tblMangrove
(
    _id varchar(36) not null primary key,

    _name nvarchar(50) not null,
    _nameVI nvarchar(50) not null,
    _nameEN nvarchar(50) not null,

    _otherName nvarchar(50) not null,
    _otherNameVI nvarchar(50) not null,
    _otherNameEN nvarchar(50) not null,

    _scientificName nvarchar(50) not null,
    _surname nvarchar(50) not null,
    _mainImage nvarchar(max) not null,

    _morphology nvarchar(max) not null,
    _morphologyVI nvarchar(max) not null,
    _morphologyEN nvarchar(max) not null,

    _ecology nvarchar(max) not null,
    _ecologyVI nvarchar(max) not null,
    _ecologyEN nvarchar(max) not null,

    _distribution nvarchar(256) not null,
    _distributionVI nvarchar(256) not null,
    _distributionEN nvarchar(256) not null,

    _conservationStatus nvarchar(256) not null,
    _conservationStatusVI nvarchar(256) not null,
    _conservationStatusEN nvarchar(256) not null,

    _use nvarchar(max) not null,
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

    _position nvarchar(256) not null,
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
    
    _name nvarchar(100) not null,
    _nameVI nvarchar(100) not null,
    _nameEN nvarchar(100) not null,
    
    _idIndividual varchar(36) foreign key references tblIndividual(_id),
    _surveyDay datetime not null,
    _mainImage nvarchar(max) not null,
)

-- Bảng lưu trữ ảnh
create table tblPhotos
(
    _id varchar(36) not null primary key,
    _idObj varchar(36) not null,
    _imageName nvarchar(256) not null,

    _noteImg nvarchar(256) null,
    _noteImgVI nvarchar(256) null,
    _noteImgEN nvarchar(256) null,
)