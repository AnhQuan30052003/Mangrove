create database mangrove;
use mangrove;

use master;
drop database mangrove;

--- [Drop tables] --
drop table if exists tblHome;

drop table if exists tblStage;
drop table if exists tblIndividual;
drop table if exists tblMangrove;

drop table if exists tblPhotos;


-- [Add tables] --
-- Bảng cho trang home "tìm kiếm"
create table tblHome
(
    _footerImg nvarchar(256) not null,
    _timeWork_open time not null,
    _timeWork_close time not null,
    _yearStart int not null,
    _yearEnd int not null
)

-- Bảng tổng quan cây ngập mặn
create table tblMangrove
(
    _id varchar(36) not null primary key,
    _name nvarchar(50) not null,
    _otherName nvarchar(50) not null,
    _scientificName nvarchar(50) not null,
    _surname nvarchar(50) not null,
    _mainImage nvarchar(max) not null,
    _morphology nvarchar(max) not null,
    _ecology nvarchar(max) not null,
    _distribution nvarchar(256) not null,
    _conservationStatus nvarchar(256) not null,
    _use nvarchar(max) not null,
    _quantity int not null,
    _view int not null,
    _updateLast datetime not null
)

-- Bảng cá thể
create table tblIndividual
(
    _id varchar(36) not null primary key,
    _idMangrove varchar(36) foreign key references tblMangrove(_id),
    _number int not null,
    _surveyDay datetime not null
)

-- Bảng giai đoạn
create table tblStage
(
    _id varchar(36) not null primary key,
    _idIndividual varchar(36) foreign key references tblIndividual(_id)
)

-- Bảng lưu trữ ảnh
create table tblPhotos
(
    _id varchar(36) not null primary key,
    _idObj varchar(36) not null,
    _imageNameId nvarchar(max) not null,
    _noteImg nvarchar(256) not null,
)