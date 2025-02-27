use mangrove

-- [Delete data from tables]
-- delete tblHome
delete tblStage
delete tblIndividual
delete tblMangrove
delete tblPhotos

-- [Insert data for tables]
-- insert into tblHome
-- values
--     (N'', '07:30:00', '17:30:00', 2023, 2025)

insert into tblMangrove
values
    (
        '00000000-AAAA-AAAA-AAAA-AAAAAAAAA000',
        N'Ráng Đại', N'Ráng vàng, Ráng biển',
        N'Acrostichum aureum L.', N'Pteridaceae',
        N'00000000-AAAA-AAAA-AAAA-AAAAAAAAP000.jpg',
        N'Loài dương xỉ trên cạn lớn, mọc thành cụm, cao tới 3 m. Thân ngắn, phủ nhiều vảy lớn, và chồi mọc sát gốc. Lá kép lông chim, cao trên 2m, khoảng 30 lá chét có phiến dày.
        Sinh sản vô tính bằng bào tử ở mặt dưới các “lá chét sinh sản”. Lá chét sinh sản có mặt trên màu xanh lục, mặt dưới nhám mang các ổ bào tử màu nâu vàng.',
        N'Đó là một loài dương xỉ lâu năm, phổ biến nhất được tìm thấy phía sau rừng ngập mặn. Thường mọc hoang ở vùng trũng, bờ rạch, bên mép đầm lầy ngập mặn, nước lợ hay trên đất có nhiều mùn, đôi khi trên bãi cát. Tái sinh tự nhiên mạnh bằng bào tử.
        Loài cây ngập mặn thực sự.',
        N'Khá phổ biến ở khu vực ven biển Bắc Khánh Hòa.',
        N'Không có nguy cơ.',
        N'Đọt non luộc ăn được.
        Lá khô dùng làm chổi (sóng lá), các sản phẩm thủ công mỹ nghệ, hoặc lợp mái nhà.',
        0,
        0,
        '2025-02-15'
    ),
    (
        '00000000-AAAA-AAAA-AAAA-AAAAAAAAA001',
        N'Vẹt dù', N'Vẹt rễ lồi, Vẹt hoa đỏ',
        N'Bruguiera gymnorhiza (L.) Lamk.', N'Rhizophoraceae',
        N'00000000-AAAA-AAAA-AAAA-AAAAAAAAP006.jpg',
        N'Cây gỗ trung bình, cao đến 15cm, DBH: 30cm. Các rễ mọc ngang dưới lòng đất phát triển thành rễ đầu gối nổi lên trên.
        Lá đơn, mọc đối, hình xoan, lá kèm hình búp màu đỏ, dài 5cm.
        Hoa đơn độc ở nách lá, dài 4-5cm. Đài hoa: 9-14 thùy, màu đỏ, hình chuông, tràng hoa 9-14 cánh màu cam và có lông, nhị gồm 9-14 cặp.
        Trụ mầm phát triển từ bên trong ống đài, có dạng xì gà, dài 20cmm, khi chín màu nâu.
        Mùa hoa, quả: từ tháng 4-12.',
        N'Thường mọc ở nơi có bùn dọc bờ biển, vùng đất ngập triều, hoặc có thể mọc trên đất khô mặn.
        Loài cây ngập mặn thực sự.',
        N'Hiện diện rải rác ở ven đầm Nha Phu và đầm Môn.',
        N'Sinh cảnh của loài đang bị thu hẹp, cần gây trồng bảo tồn.',
        N'Gỗ cứng, tốt, dùng trong xây dựng. Vỏ cây chứa nhiều tanin, vị chát, dùng để nhuộm vải, lưới và thuộc da.
        Trụ mầm có thể ăn được.
        Theo YHCT: Vỏ cây dùng để điều trị bệnh sốt rét, tiêu chảy.',
        0,
        0,
        '2025-02-15'
    ),
    (
        '00000000-AAAA-AAAA-AAAA-AAAAAAAAA002',
        N'Dà vôi', N'Dà đỏ',
        N'Ceriops tagal (Perr.) C. B. Roxb.', N'Rhizophoraceae',
        N'00000000-AAAA-AAAA-AAAA-AAAAAAAAP009.png',
        N'Cây gỗ nhỏ hoặc cây bụi, cao đến 6-8m, DBH: 20cm, vỏ thân màu xám nâu. Thường có rễ cà kheo nhỏ.
        Lá đơn, mọc đối, hình trứng ngược, mép nguyên. Lá kèm dẹp.
        Hoa tự xim ở nách lá, gồm 5–10 hoa nhỏ. Tràng hoa gồm 5 cánh màu trắng, sau chuyển sang nâu cam, có 3 phụ bộ dạng sợi, nhị 10.
        Quả hình trứng dài 1,5 cm mọc thòng từ ống đài.
        Trụ mầm thuôn dài 20cm và có sọc.
        Mùa hoa quả: tháng 3-8.',
        N'Cây sinh trưởng chậm, tái sinh chủ yếu bằng trụ mầm. Thích hợp trên đất phù sa, bãi bùn ngập triều ở cửa sông.
        Loài cây ngập mặn thực sự.',
        N'Phân bố hẹp ở ven đầm Nha Phu và Hòn Lớn (Vạn Ninh).',
        N'Loài sẽ có nguy cơ do môi trường sống bị thu hẹp, cần gây trồng bảo tồn.',
        N'Gỗ màu đỏ, bền, dùng làm mộc dân dụng, trang trí nội thất, than củi. Vỏ cây có nhiều tannin dùng làm thuốc nhuộm.
        Theo YHCT: Dùng nước sắc từ chồi cây Dà để chữa sốt rét,...',
        0,
        0,
        '2025-02-15'
    ),
    (
        '00000000-AAAA-AAAA-AAAA-AAAAAAAAA003',
        N'Bần trắng', N'',
        N'Sonneratia alba Smith.', N'Sonneratiaceae (Lythraceae)',
        N'00000000-AAAA-AAAA-AAAA-AAAAAAAAP014.png',
        N'Cây gỗ lớn, cao 15m, DBH: 1m vỏ thân màu nâu sậm, nứt dọc. nhiều rễ thở hình măng (dạng hình măng).
        Lá đơn, mọc đối, hình trứng ngược, đầu lá tròn. 
        Hoa mọc thành chùm, 5-8 hoa màu trắng, thường nở vào ban đêm. 
        đài 6-7 tràng 6-7 nhị nhiều màu trắng bầu 14-16 ngăn. 
        Quả thịt, hình bánh xe, rộng 3,5cm, có đài tồn tại phía gốc, với hơn 100 hạt. 
        Mùa hoa quả: tháng 4-11.',
        N'Mọc ở khu vực bán ngập triều, thích hợp độ mặn cao, nơi có hỗn hợp đất bùn và cát. Bần trắng là loài cây tiên phong, thường chiếm ưu thế (tạo thành quần thể) phát triển nhanh.   
        Loài cây ngập mặn thực sự.',
        N'Hiện diện ở đầm Nha Phu và đầm Môn và hòn Lớn.',
        N'Do thường bị chặt phá nên phạm vi phân bố đang thu hẹp dần, cần được bảo vệ và gây trồng phục hồi.',
        N'Gỗ được sử dụng để xây dựng nhà và đóng thuyền. 
        Rễ dạng măng dùng để làm nút chai và phao.
        Theo YHCT: Dùng chữa sưng và bong gân.',
        0,
        0,
        '2025-02-22'
    )

insert into tblIndividual
values
    ('00000000-AAAA-AAAA-AAAA-AAAAAAAAI000', '00000000-AAAA-AAAA-AAAA-AAAAAAAAA003', N'Vạn Giã, Vạn Ninh, Khánh Hoà', '2025-02-20', 'qr-code.png', 0),
    ('00000000-AAAA-AAAA-AAAA-AAAAAAAAI001', '00000000-AAAA-AAAA-AAAA-AAAAAAAAA002', N'Vạn Giã, Vạn Ninh, Khánh Hoà', '2025-02-21', 'qr-code.png', 0)

insert into tblStage
values
    ('00000000-AAAA-AAAA-AAAA-AAAAAAAAS000', N'Tên giai đoạn 1', '00000000-AAAA-AAAA-AAAA-AAAAAAAAI000', '2025-02-25', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAB016.jpg'),
    ('00000000-AAAA-AAAA-AAAA-AAAAAAAAS001', N'Tên giai đoạn 1', '00000000-AAAA-AAAA-AAAA-AAAAAAAAI001', '2025-02-26', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAB020.jpg')

insert into tblPhotos
values
    ('00000000-AAAA-AAAA-AAAA-AAAAAAAAB000', '00000000-AAAA-AAAA-AAAA-AAAAAAAAA000', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAP000.jpg', N'Ghi chú 1'),
    ('00000000-AAAA-AAAA-AAAA-AAAAAAAAB001', '00000000-AAAA-AAAA-AAAA-AAAAAAAAA000', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAP001.jpg', N'Ghi chú 2'),
    ('00000000-AAAA-AAAA-AAAA-AAAAAAAAB002', '00000000-AAAA-AAAA-AAAA-AAAAAAAAA001', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAP002.jpg', N'Ghi chú 1'),
    ('00000000-AAAA-AAAA-AAAA-AAAAAAAAB003', '00000000-AAAA-AAAA-AAAA-AAAAAAAAA001', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAP003.png', N'Ghi chú 2'),
    ('00000000-AAAA-AAAA-AAAA-AAAAAAAAB004', '00000000-AAAA-AAAA-AAAA-AAAAAAAAA001', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAP004.jpg', N'Ghi chú 3'),
    ('00000000-AAAA-AAAA-AAAA-AAAAAAAAB005', '00000000-AAAA-AAAA-AAAA-AAAAAAAAA001', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAP005.jpg', N'Ghi chú 4'),
    ('00000000-AAAA-AAAA-AAAA-AAAAAAAAB006', '00000000-AAAA-AAAA-AAAA-AAAAAAAAA001', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAP006.jpg', N'Ghi chú 5'),
    ('00000000-AAAA-AAAA-AAAA-AAAAAAAAB007', '00000000-AAAA-AAAA-AAAA-AAAAAAAAA002', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAP007.jpg', N'Ghi chú 1'),
    ('00000000-AAAA-AAAA-AAAA-AAAAAAAAB008', '00000000-AAAA-AAAA-AAAA-AAAAAAAAA002', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAP008.jpg', N'Ghi chú 2'),
    ('00000000-AAAA-AAAA-AAAA-AAAAAAAAB009', '00000000-AAAA-AAAA-AAAA-AAAAAAAAA002', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAP009.png', N'Ghi chú 3'),
    ('00000000-AAAA-AAAA-AAAA-AAAAAAAAB010', '00000000-AAAA-AAAA-AAAA-AAAAAAAAA002', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAP010.jpg', N'Ghi chú 4'),
    ('00000000-AAAA-AAAA-AAAA-AAAAAAAAB011', '00000000-AAAA-AAAA-AAAA-AAAAAAAAA003', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAP011.jpg', N'Ghi chú 1'),
    ('00000000-AAAA-AAAA-AAAA-AAAAAAAAB012', '00000000-AAAA-AAAA-AAAA-AAAAAAAAA003', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAP012.jpg', N'Ghi chú 2'),
    ('00000000-AAAA-AAAA-AAAA-AAAAAAAAB013', '00000000-AAAA-AAAA-AAAA-AAAAAAAAA003', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAP013.jpg', N'Ghi chú 3'),
    ('00000000-AAAA-AAAA-AAAA-AAAAAAAAB014', '00000000-AAAA-AAAA-AAAA-AAAAAAAAA003', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAP014.png', N'Ghi chú 4'),
    ('00000000-AAAA-AAAA-AAAA-AAAAAAAAB015', '00000000-AAAA-AAAA-AAAA-AAAAAAAAA003', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAP015.jpg', N'Ghi chú 5'),


    ('00000000-AAAA-AAAA-AAAA-AAAAAAAAB016', '00000000-AAAA-AAAA-AAAA-AAAAAAAAS000', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAB016.jpg', N''),
    ('00000000-AAAA-AAAA-AAAA-AAAAAAAAB017', '00000000-AAAA-AAAA-AAAA-AAAAAAAAS000', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAB017.jpg', N'Sinh cảnh'),
    ('00000000-AAAA-AAAA-AAAA-AAAAAAAAB018', '00000000-AAAA-AAAA-AAAA-AAAAAAAAS000', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAB018.jpg', N'Bộ rễ'),
    ('00000000-AAAA-AAAA-AAAA-AAAAAAAAB019', '00000000-AAAA-AAAA-AAAA-AAAAAAAAS000', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAB019.jpg', N'Quẩn thể'),

    ('00000000-AAAA-AAAA-AAAA-AAAAAAAAB020', '00000000-AAAA-AAAA-AAAA-AAAAAAAAS001', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAB020.jpg', N''),
    ('00000000-AAAA-AAAA-AAAA-AAAAAAAAB021', '00000000-AAAA-AAAA-AAAA-AAAAAAAAS001', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAB021.jpg', N'Trụ mầm và hoa'),
    ('00000000-AAAA-AAAA-AAAA-AAAAAAAAB022', '00000000-AAAA-AAAA-AAAA-AAAAAAAAS001', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAB022.jpg', N'Trụ mầm và hoa')