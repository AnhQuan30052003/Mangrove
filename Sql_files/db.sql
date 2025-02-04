use mangrove;

-- Bảng cho trang home "tìm kiếm"
create table tblHome
(
    _logo nvarchar(256) not null,
    _footer nvarchar(256) not null,
    _itemsSlider int not null,
    _timeWork_open time not null,
    _timeWork_close time not null,
)

-- Bảng tỉnh
create table tblProvince
(
    _id varchar(36) not null primary key,
    _name nvarchar(50) not null,
)

-- Bảng huyện
create table tblDistrict
(
    _id varchar(36) not null primary key,
    _name nvarchar(50) not null,
)

-- Bảng tỉnh - huyện
create table tblProvince_District
(
    _id varchar(36) not null primary key,
    _idProvince varchar(36) foreign key references tblProvince(_id),
    _idDistrict varchar(36) foreign key references tblDistrict(_id),
)

-- Bảng tổng quan cây ngập mặn
create table tblMangrove
(
    _id varchar(36) not null primary key,
    _name nvarchar(50) not null,
    _otherName nvarchar(50) not null,
    _scientificName nvarchar(50) not null,
    _surname nvarchar(50) not null,
    _morphology text not null,
    _ecology text not null,
    _distribution nvarchar(256) not null,
    _conservationStatus nvarchar(256) not null,
    _use text not null,
)

-- Bảng giai đoạn đo
create table tblStage
(
    _id varchar(36) not null primary key,
    _surveyDay datetime not null,
    _qrImgName varchar(36) not null,
)

-- Bảng tổng quan cây & giai đoạn
create table tblMangrove_Stage
(
    _id varchar(36) not null primary key,
    _idMangrove varchar(36) foreign key references tblMangrove(_id),
    _idState varchar(36) foreign key references tblStage(_id),
)