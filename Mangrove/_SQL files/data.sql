create database mangrove;
use mangrove;

-- [Delete data from tables]
delete tblHome;
delete tblMangrove;
delete tblPhotos;

-- [Insert data for tables]
insert into tblHome
values (N'', '07:30:00', '17:30:00', 2023, 2025);

insert into tblMangrove values
(
    '00000000-AAAA-AAAA-AAAA-AAAAAAAAA000',
    N'Ráng Đại', N'Ráng vàng, Ráng biển',
    N'Acrostichum aureum L.', N'Pteridaceae',
    N'00000000-AAAA-AAAA-AAAA-AAAAAAAAP000.jpg',
    N'Loài dương xỉ trên cạn lớn, mọc thành cụm, cao tới 3 m. Thân ngắn, phủ nhiều vảy lớn, và chồi mọc sát gốc. Lá kép lông chim, cao trên 2m, khoảng 30 lá chét có phiến dày.<br />Sinh sản vô tính bằng bào tử ở mặt dưới các “lá chét sinh sản”. Lá chét sinh sản có mặt trên màu xanh lục, mặt dưới nhám mang các ổ bào tử màu nâu vàng.',
    N'Đó là một loài dương xỉ lâu năm, phổ biến nhất được tìm thấy phía sau rừng ngập mặn. Thường mọc hoang ở vùng trũng, bờ rạch, bên mép đầm lầy ngập mặn, nước lợ hay trên đất có nhiều mùn, đôi khi trên bãi cát. Tái sinh tự nhiên mạnh bằng bào tử.<br />Loài cây ngập mặn thực sự.',
    N'Khá phổ biến ở khu vực ven biển Bắc Khánh Hòa.',
    N'Không có nguy cơ.',
    N'Đọt non luộc ăn được.<br>Lá khô dùng làm chổi (sóng lá), các sản phẩm thủ công mỹ nghệ, hoặc lợp mái nhà.',
    0,
    7,
    '2025-02-15'
),
(
    '00000000-AAAA-AAAA-AAAA-AAAAAAAAA001',
    N'Vẹt dù', N'Vẹt rễ lồi, Vẹt hoa đỏ',
    N'Bruguiera gymnorhiza (L.) Lamk.', N'Rhizophoraceae',
    N'00000000-AAAA-AAAA-AAAA-AAAAAAAAP006.jpg',
    N'Cây gỗ trung bình, cao đến 15cm, DBH: 30cm. Các rễ mọc ngang dưới lòng đất phát triển thành rễ đầu gối nổi lên trên.<br />Lá đơn, mọc đối, hình xoan; lá kèm hình búp màu đỏ, dài 5cm.<br />Hoa đơn độc ở nách lá, dài 4-5cm. Đài hoa: 9-14 thùy, màu đỏ, hình chuông; tràng hoa 9-14 cánh màu cam và có lông; nhị gồm 9-14 cặp.<br/>Trụ mầm phát triển từ bên trong ống đài, có dạng xì gà, dài 20cmm, khi chín màu nâu.<br/>Mùa hoa, quả: từ tháng 4-12.',
    N'Thường mọc ở nơi có bùn dọc bờ biển, vùng đất ngập triều, hoặc có thể mọc trên đất khô mặn.<br/>Loài cây ngập mặn thực sự.',
    N'Hiện diện rải rác ở ven đầm Nha Phu và đầm Môn.',
    N'Sinh cảnh của loài đang bị thu hẹp, cần gây trồng bảo tồn.',
    N'Gỗ cứng, tốt, dùng trong xây dựng. Vỏ cây chứa nhiều tanin, vị chát; dùng để nhuộm vải, lưới và thuộc da.<br/>Trụ mầm  có thể ăn được.<br/>Theo YHCT: Vỏ cây dùng để điều trị bệnh sốt rét, tiêu chảy.',
    0,
    3,
    '2025-02-15'
),
(
    '00000000-AAAA-AAAA-AAAA-AAAAAAAAA002',
    N'Dà vôi', N'Dà đỏ',
    N'Ceriops tagal (Perr.) C. B. Roxb.', N'Rhizophoraceae',
    N'00000000-AAAA-AAAA-AAAA-AAAAAAAAP009.png',
    N'Cây gỗ nhỏ hoặc cây bụi, cao đến 6-8m, DBH: 20cm; vỏ thân màu xám nâu. Thường có rễ cà kheo nhỏ.<br/>Lá đơn, mọc đối, hình trứng ngược, mép nguyên. Lá kèm dẹp.<br/>Hoa tự xim ở nách lá, gồm 5–10 hoa nhỏ. Tràng hoa gồm 5 cánh màu trắng, sau chuyển sang nâu cam, có 3 phụ bộ dạng sợi; nhị 10.<br/>Quả hình trứng dài 1,5 cm mọc thòng từ ống đài.<br/>Trụ mầm thuôn dài 20cm và có sọc.<br/>Mùa hoa quả: tháng 3-8.',
    N'Cây sinh trưởng chậm, tái sinh chủ yếu bằng trụ mầm. Thích hợp trên đất phù sa, bãi bùn ngập triều ở cửa sông.<br/>Loài cây ngập mặn thực sự.',
    N'Phân bố hẹp ở ven đầm Nha Phu và Hòn Lớn (Vạn Ninh).',
    N'Loài sẽ có nguy cơ do môi trường sống bị thu hẹp, cần gây trồng bảo tồn.',
    N'Gỗ màu đỏ, bền, dùng làm mộc dân dụng, trang trí nội thất, than củi. Vỏ cây có nhiều tannin dùng làm thuốc nhuộm.<br/>Theo YHCT: Dùng nước sắc từ chồi cây Dà để chữa sốt rét,...',
    0,
    10,
    '2025-02-15'
);

insert into tblPhotos values
('00000000-AAAA-AAAA-AAAA-AAAAAAAAB000', '00000000-AAAA-AAAA-AAAA-AAAAAAAAA000', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAP000.jpg'),
('00000000-AAAA-AAAA-AAAA-AAAAAAAAB001', '00000000-AAAA-AAAA-AAAA-AAAAAAAAA000', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAP001.jpg'),
('00000000-AAAA-AAAA-AAAA-AAAAAAAAB002', '00000000-AAAA-AAAA-AAAA-AAAAAAAAA001', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAP002.jpg'),
('00000000-AAAA-AAAA-AAAA-AAAAAAAAB003', '00000000-AAAA-AAAA-AAAA-AAAAAAAAA001', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAP003.png'),
('00000000-AAAA-AAAA-AAAA-AAAAAAAAB004', '00000000-AAAA-AAAA-AAAA-AAAAAAAAA001', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAP004.jpg'),
('00000000-AAAA-AAAA-AAAA-AAAAAAAAB005', '00000000-AAAA-AAAA-AAAA-AAAAAAAAA001', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAP005.jpg'),
('00000000-AAAA-AAAA-AAAA-AAAAAAAAB006', '00000000-AAAA-AAAA-AAAA-AAAAAAAAA001', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAP006.jpg'),
('00000000-AAAA-AAAA-AAAA-AAAAAAAAB007', '00000000-AAAA-AAAA-AAAA-AAAAAAAAA002', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAP007.jpg'),
('00000000-AAAA-AAAA-AAAA-AAAAAAAAB008', '00000000-AAAA-AAAA-AAAA-AAAAAAAAA002', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAP008.jpg'),
('00000000-AAAA-AAAA-AAAA-AAAAAAAAB009', '00000000-AAAA-AAAA-AAAA-AAAAAAAAA002', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAP009.png'),
('00000000-AAAA-AAAA-AAAA-AAAAAAAAB010', '00000000-AAAA-AAAA-AAAA-AAAAAAAAA002', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAP010.jpg');