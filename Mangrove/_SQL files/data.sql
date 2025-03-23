use mangrove

-- [Delete data from tables]
delete tblHome
delete tblSetting

delete tblStage
delete tblIndividual
delete tblMangrove

delete tblPhotos
delete tblDistributiton

-- [Insert data for tables
insert into tblSetting
values
    (
        N'logo.png',
        N'bg-footer.jpg',
        N'0398 090 114',
        N'mangrove.ntu.edu.vn',

        N'Đại học Nha Trang',
        N'Nha Trang University',

        N'Viện Công Nghệ Sinh Học & Môi Trường',
        N'Institute of Biotechnology & Environment',

        N'Website cung cấp các thông tin chi tiết, liên quan đến các loài cây ngập mặn ở phía Bắc - Khánh Hoà.',
        N'Website provides detailed information related to mangrove species in the North - Khanh Hoa.',

        N'02 Nguyễn Đình Chiểu, Vĩnh Thọ, Nha Trang, Khánh Hoà',
        N'02 Nguyen Dinh Chieu, Vinh Tho, Nha Trang, Khanh Hoa'
    )

insert into tblHome
values
    (
        N'banner.jpg',
        6,

        N'Cây Ngập Mặn Tại Bắc - Khánh Hoà.',
        N'Mangrove Trees In The North - Khanh Hoa.',

        N'Trang web cung cấp thông tin chi tiết về hệ sinh thái rừng ngập mặn tại Bắc – Khánh Hòa, giúp nâng cao nhận thức và kêu gọi cộng đồng chung tay bảo vệ môi trường.
        Chúng tôi chia sẻ kiến thức về các loài cây ngập mặn, vai trò quan trọng của chúng, thực trạng hiện tại và những hoạt động bảo tồn đang diễn ra.
        Hãy cùng chung tay gìn giữ rừng ngập mặn để bảo vệ hệ sinh thái ven biển bền vững !',
        N'The website provides detailed information about the mangrove ecosystem in Bac – Khanh Hoa, helping to raise awareness and call on the community to join hands to protect the environment.
        We share knowledge about mangrove species, their important roles, current status and ongoing conservation activities.
        Let''s join hands to preserve mangroves to protect sustainable coastal ecosystems!'
    )

insert into tblMangrove
values
    (
        '00000000-AAAA-AAAA-AAAA-AAAAAAAAA000',

        N'Ráng đại',
        N'Acrostichum aureum L.',

        N'Ráng vàng, Ráng biển',
        N'Golden leather fern, mangrove fern',

        N'Acrostichum aureum L.',
        N'Pteridaceae',
        N'00000000-AAAA-AAAA-AAAA-AAAAAAAAP000_Ráng đại.jpg',

        N'Loài dương xỉ trên cạn lớn, mọc thành cụm, cao tới 3 m. Thân ngắn, phủ nhiều vảy lớn, và chồi mọc sát gốc. Lá kép lông chim, cao trên 2m, khoảng 30 lá chét có phiến dày.
        Sinh sản vô tính bằng bào tử ở mặt dưới các “lá chét sinh sản”. Lá chét sinh sản có mặt trên màu xanh lục, mặt dưới nhám mang các ổ bào tử màu nâu vàng.',
        N'Large, clump-forming terrestrial fern, up to 3 m tall. Short stem, covered with large scales, and many buds close to the base. Leaves pinnate, over 2m tall, about 30 leaflets, with thick blades. 
        Asexual reproduction by spores on the underside of "reproductive leaflets". Reproductive leaflets have a green upper surface and a rough lower surface bearing yellow-brown spore foci.',

        N'Đó là một loài dương xỉ lâu năm, phổ biến nhất được tìm thấy phía sau rừng ngập mặn. Thường mọc hoang ở vùng trũng, bờ rạch, bên mép đầm lầy ngập mặn, nước lợ hay trên đất có nhiều mùn, đôi khi trên bãi cát. Tái sinh tự nhiên mạnh bằng bào tử.
        Loài cây ngập mặn thực sự.',
        N'It is a perennial fern, the most common species found on the inland side of mangroves, often grows wild in low-lying areas, canal banks, on the edge of mangrove swamps, brackish water or on humus-rich soil, sometimes on sandy beaches.
        Strong natural regeneration by spores. 
        True mangrove species.',

        N'Khá phổ biến ở khu vực ven biển Bắc Khánh Hòa.',
        N'Quite popular in the coastal area of Northern Khanh Hoa.',

        N'Không có nguy cơ.',
        N'No risk.',

        N'Đọt non luộc ăn được.
        Lá khô dùng làm chổi (sóng lá), các sản phẩm thủ công mỹ nghệ, hoặc lợp mái nhà.',
        N'The young shoots can be boiled and eaten.
        The dried leaves are used to make brooms, handicraft products, or for thatching roofs.',

        0,
        '2025-03-05'
    ),
    (
        '00000000-AAAA-AAAA-AAAA-AAAAAAAAA001',

        N'Vẹt dù',
        N'Bruguiera gymnorhiza (L.) Lamk.',

        N'Vẹt rễ lồi, Vẹt hoa đỏ',
        N'Orange mangrove, Caribbean mangrove',

        N'Bruguiera gymnorhiza (L.) Lamk.',
        N'Rhizophoraceae',
        N'00000000-AAAA-AAAA-AAAA-AAAAAAAAP006_Vẹt dù.jpg',

        N'Cây gỗ trung bình, cao đến 15cm, DBH: 30cm. Các rễ mọc ngang dưới lòng đất phát triển thành rễ đầu gối nổi lên trên.
        Lá đơn, mọc đối, hình xoan, lá kèm hình búp màu đỏ, dài 5cm.
        Hoa đơn độc ở nách lá, dài 4-5cm. Đài hoa: 9-14 thùy, màu đỏ, hình chuông, tràng hoa 9-14 cánh màu cam và có lông, nhị gồm 9-14 cặp.
        Trụ mầm phát triển từ bên trong ống đài, có dạng xì gà, dài 20cmm, khi chín màu nâu.
        Mùa hoa, quả: từ tháng 4-12.',
        N'Medium-sized tree up to 15m tall, DBH: 30cm. Roots that grow horizontally underground develop into knee roots that emerge above.
        Simple leaves, opposite, elliptical in shape, the stipules are reddish in color, 5 cm long.
        Single axillary flowers, 4-5cm. Calyx: 9-14 lobes, red in color, bell-shapedy, 9-14 orange and hairy petals, 9-14 pairs of stamens.
        The propagule shape is cylindrical and cigar-shaped, 20cm long, brown when ripe, it grows within the calyx tube.
        Flowering & Fruit: April-December.',

        N'Thường mọc ở nơi có bùn dọc bờ biển, vùng đất ngập triều, hoặc có thể mọc trên đất khô mặn.
        Loài cây ngập mặn thực sự.',
        N'Usually grows in muddy areas along the coast, tidal lands, or can also grow on dry saline soil.
        True mangrove species.',

        N'Hiện diện rải rác ở ven đầm Nha Phu và đầm Môn.',
        N'Scattered distribution along the edges of Nha Phu and Môn lagoons.',

        N'Sinh cảnh của loài đang bị thu hẹp, cần gây trồng bảo tồn.',
        N'The species'' habitat is shrinking and needs conservation planting.',

        N'Gỗ cứng, tốt, dùng trong xây dựng. Vỏ cây chứa nhiều tanin, vị chát, dùng để nhuộm vải, lưới và thuộc da.
        Trụ mầm có thể ăn được.
        Theo YHCT: Vỏ cây dùng để điều trị bệnh sốt rét, tiêu chảy.',
        N'The wood is hard and good quality, used in construction. The bark contains a lot of tannins, with an astringent taste, it is used for dyeing fabrics, nets, and tanning leather.
        The propagules can be eaten.
        Traditional medicine: The bark is used to treat malaria, diarrhea.',

        0,
        '2025-02-15'
    ),
    (
        '00000000-AAAA-AAAA-AAAA-AAAAAAAAA002',

        N'Dà vôi',
        N'Ceriops tagal (Perr.) C. B. Roxb.',

        N'Dà đỏ',
        N'Spurred mangrove, Rib-fruited yellow mangrove',

        N'Ceriops tagal (Perr.) C. B. Roxb.',
        N'Rhizophoraceae',
        N'00000000-AAAA-AAAA-AAAA-AAAAAAAAP009_Dà vôi.png',

        N'Cây gỗ nhỏ hoặc cây bụi, cao đến 6-8m, DBH: 20cm, vỏ thân màu xám nâu. Thường có rễ cà kheo nhỏ.
        Lá đơn, mọc đối, hình trứng ngược, mép nguyên. Lá kèm dẹp.
        Hoa tự xim ở nách lá, gồm 5–10 hoa nhỏ. Tràng hoa gồm 5 cánh màu trắng, sau chuyển sang nâu cam, có 3 phụ bộ dạng sợi, nhị 10.
        Quả hình trứng dài 1,5 cm mọc thòng từ ống đài.
        Trụ mầm thuôn dài 20cm và có sọc.
        Mùa hoa quả: tháng 3-8.',
        N'Small tree or shrub up to 6-8 m tall, DBH: 20cn, with a brown - grey bark. The tree often has small stilt roots.
        Simple leaves, opposite, obovate with entire margins, stipule flattened knife-like.
        Inflorescence: cyme with 5-10 small flowers.
        The flower has 5 white petals and soon turn brown, with 3 fibrous lobes at the tip of the petal, calyx: 5 lobes, stamen: 10.
        The ovoid fruits are up to 1.5 cm long, suspended from the calyx tube. The hypocotyl is long and slender, up to 20 cm, ribbed. 
        Flowering & Fruit: March-August.',

        N'Cây sinh trưởng chậm, tái sinh chủ yếu bằng trụ mầm. Thích hợp trên đất phù sa, bãi bùn ngập triều ở cửa sông.
        Loài cây ngập mặn thực sự.',
        N'The tree grows slowly, regenerating mainly by propagules. Suitable on alluvial soils and tidal mud flats at river mouths.
        True mangrove species.',

        N'Phân bố hẹp ở ven đầm Nha Phu và Hòn Lớn (Vạn Ninh).',
        N'Narrow distribution range along the edge of Nha Phu lagoon and in Hòn Lớn (Van Ninh).',

        N'Loài sẽ có nguy cơ do môi trường sống bị thu hẹp, cần gây trồng bảo tồn.',
        N'The species will be at risk due to shrinking habitat and needs conservation planting.',

        N'Gỗ màu đỏ, bền, dùng làm mộc dân dụng, trang trí nội thất, than củi. Vỏ cây có nhiều tannin dùng làm thuốc nhuộm.
        Theo YHCT: Dùng nước sắc từ chồi cây Dà để chữa sốt rét,...',
        N'The durable wood used for civil carpentry, interior decoration, and charcoal. The bark has a lot of tannin used as a dye.
        Traditional medicine: Decoction from leaf buds is used to treat malaria.',

        0,
        '2025-02-15'
    ),
    (
        '00000000-AAAA-AAAA-AAAA-AAAAAAAAA003',

        N'Bần trắng',
        N'Sonneratia alba Smith.',

        N'',
        N'Mangrove apple',

        N'Sonneratia alba Smith.',
        N'Sonneratiaceae (Lythraceae)',
        N'00000000-AAAA-AAAA-AAAA-AAAAAAAAP014_Bần trắng.png',

        N'Cây gỗ lớn, cao 15m, DBH: 1m vỏ thân màu nâu sậm, nứt dọc. nhiều rễ thở hình măng (dạng hình măng).
        Lá đơn, mọc đối, hình trứng ngược, đầu lá tròn. 
        Hoa mọc thành chùm, 5-8 hoa màu trắng, thường nở vào ban đêm. Đài 6-7 tràng 6-7 nhị nhiều màu trắng bầu 14-16 ngăn. 
        Quả thịt, hình bánh xe, rộng 3,5cm, có đài tồn tại phía gốc, với hơn 100 hạt. 
        Mùa hoa quả: tháng 4-11.',
        N'Large tree up to 15 m tall, DBH: 1m. Bark dark brown, vertical fissures. Pneumatophores cone-shaped (shoot-shaped).
        Leaves simple, opposite, ovate, with rounded tips.
        Flowering grow in clusters, 5-8 white flowers and usually blooms at night. Calyx 6-7; petals 6-7; numerous white stamens; ovary 14-16 cells.
        Fruit: Berry 3.5 cm in diam., wheel-shaped, with a persistent calyx at the base and more than 100 small seeds.
        Flowering & Fruit: April-November.',

        N'Mọc ở khu vực bán ngập triều, thích hợp độ mặn cao, nơi có hỗn hợp đất bùn và cát. Bần trắng là loài cây tiên phong, thường chiếm ưu thế (tạo thành quần thể) phát triển nhanh.   
        Loài cây ngập mặn thực sự.',
        N'Grows in semi-tidal areas, suitable for high salinity, where there is a mixture of mud and sand. It is a pioneer tree species, often dominant (forming a population) and grow fast.
        True mangrove species.',

        N'Hiện diện ở đầm Nha Phu và đầm Môn và hòn Lớn.',
        N'Distributed in Nha Phu lagoon, Môn lagoon and Hòn Lớn island.',

        N'Do thường bị chặt phá nên phạm vi phân bố đang thu hẹp dần, cần được bảo vệ và gây trồng phục hồi.',
        N'This species is often cut down, its distribution range is gradually narrowing. Needs to be protected and replanted.',

        N'Gỗ được sử dụng để xây dựng nhà và đóng thuyền. 
        Rễ dạng măng dùng để làm nút chai và phao.
        Theo YHCT: Dùng chữa sưng và bong gân.',
        N'The wood is used in the construction of house and boat.
        The shoot-like roots are used to make corks and floats.
        Traditional medicine: Used to treat swelling and sprains.',

        0,
        '2025-02-22'
    )

insert into tblIndividual
values
    ('00000000-AAAA-AAAA-AAAA-AAAAAAAAI000', '00000000-AAAA-AAAA-AAAA-AAAAAAAAA003', '109.338674', '12.651652', N'Vạn Giã, Vạn Ninh, Khánh Hoà', N'Van Gia, Van Ninh, Khanh Hoa', '2025-03-05', N'qr-code.png', 0),
    ('00000000-AAAA-AAAA-AAAA-AAAAAAAAI001', '00000000-AAAA-AAAA-AAAA-AAAAAAAAA002', '109.323081', '12.660845', N'Vạn Giã, Vạn Ninh, Khánh Hoà', N'Van Gia, Van Ninh, Khanh Hoa', '2025-03-05', N'qr-code.png', 0),
    ('00000000-AAAA-AAAA-AAAA-AAAAAAAAI002', '00000000-AAAA-AAAA-AAAA-AAAAAAAAA003', '109.09.16.4 E', '12.45.26.6 N', N'Tuần Lẽ, Vạn Ninh, Khánh Hoà', N'Tuan Le, Van Ninh, Khanh Hoa', '2025-03-05', N'qr-code.png', 0)

insert into tblStage
values
    ('00000000-AAAA-AAAA-AAAA-AAAAAAAAS000', '00000000-AAAA-AAAA-AAAA-AAAAAAAAI000', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAB016_Bần trắng.jpg', '2025-02-15', N'Tên giai đoạn 1', N'Stage name 1', N'Trời nắng', N'Sunny'),
    ('00000000-AAAA-AAAA-AAAA-AAAAAAAAS001', '00000000-AAAA-AAAA-AAAA-AAAAAAAAI001', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAB020_Dà vôi.jpg', '2025-02-15', N'Tên giai đoạn 1', N'Stage name 1', N'Trời nắng', N'Sunny'),
    ('00000000-AAAA-AAAA-AAAA-AAAAAAAAS002', '00000000-AAAA-AAAA-AAAA-AAAAAAAAI002', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAB025_Bần trắng.jpg', '2025-02-26', N'Tên giai đoạn 1', N'Stage name 1', N'Trời nắng', N'Sunny')

insert into tblPhotos
values
    ('00000000-AAAA-AAAA-AAAA-AAAAAAAAB000', '00000000-AAAA-AAAA-AAAA-AAAAAAAAA000', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAP000_Ráng đại 0.jpg', N'Ghi chú 1', N'Note 1'),
    ('00000000-AAAA-AAAA-AAAA-AAAAAAAAB001', '00000000-AAAA-AAAA-AAAA-AAAAAAAAA000', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAP001_Ráng đại 1.jpg', N'Ghi chú 2', N'Note 2'),

    ('00000000-AAAA-AAAA-AAAA-AAAAAAAAB002', '00000000-AAAA-AAAA-AAAA-AAAAAAAAA001', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAP002_Vẹt dù.jpg', N'Ghi chú 1', N'Note 1'),
    ('00000000-AAAA-AAAA-AAAA-AAAAAAAAB003', '00000000-AAAA-AAAA-AAAA-AAAAAAAAA001', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAP003_Vẹt dù.png', N'Ghi chú 2', N'Note 2'),
    ('00000000-AAAA-AAAA-AAAA-AAAAAAAAB004', '00000000-AAAA-AAAA-AAAA-AAAAAAAAA001', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAP004_Vẹt dù.jpg', N'Ghi chú 3', N'Note 3'),
    ('00000000-AAAA-AAAA-AAAA-AAAAAAAAB005', '00000000-AAAA-AAAA-AAAA-AAAAAAAAA001', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAP005_Vẹt dù.jpg', N'Ghi chú 4', N'Note 4'),
    ('00000000-AAAA-AAAA-AAAA-AAAAAAAAB006', '00000000-AAAA-AAAA-AAAA-AAAAAAAAA001', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAP006_Vẹt dù.jpg', N'Ghi chú 5', N'Note 5'),

    ('00000000-AAAA-AAAA-AAAA-AAAAAAAAB007', '00000000-AAAA-AAAA-AAAA-AAAAAAAAA002', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAP007_Dà vôi.jpg', N'Ghi chú 1', N'Note 1'),
    ('00000000-AAAA-AAAA-AAAA-AAAAAAAAB008', '00000000-AAAA-AAAA-AAAA-AAAAAAAAA002', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAP008_Dà vôi.jpg', N'Ghi chú 2', N'Note 2'),
    ('00000000-AAAA-AAAA-AAAA-AAAAAAAAB009', '00000000-AAAA-AAAA-AAAA-AAAAAAAAA002', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAP009_Dà vôi.png', N'Ghi chú 3', N'Note 3'),
    ('00000000-AAAA-AAAA-AAAA-AAAAAAAAB010', '00000000-AAAA-AAAA-AAAA-AAAAAAAAA002', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAP010_Dà vôi.jpg', N'Ghi chú 4', N'Note 4'),

    ('00000000-AAAA-AAAA-AAAA-AAAAAAAAB011', '00000000-AAAA-AAAA-AAAA-AAAAAAAAA003', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAP011_Bần trắng.jpg', N'Ghi chú 1', N'Note 1'),
    ('00000000-AAAA-AAAA-AAAA-AAAAAAAAB012', '00000000-AAAA-AAAA-AAAA-AAAAAAAAA003', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAP012_Bần trắng.jpg', N'Ghi chú 2', N'Note 2'),
    ('00000000-AAAA-AAAA-AAAA-AAAAAAAAB013', '00000000-AAAA-AAAA-AAAA-AAAAAAAAA003', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAP013_Bần trắng.jpg', N'Ghi chú 3', N'Note 3'),
    ('00000000-AAAA-AAAA-AAAA-AAAAAAAAB014', '00000000-AAAA-AAAA-AAAA-AAAAAAAAA003', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAP014_Bần trắng.png', N'Ghi chú 4', N'Note 4'),
    ('00000000-AAAA-AAAA-AAAA-AAAAAAAAB015', '00000000-AAAA-AAAA-AAAA-AAAAAAAAA003', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAP015_Bần trắng.jpg', N'Ghi chú 5', N'Note 5'),

    ('00000000-AAAA-AAAA-AAAA-AAAAAAAAB026', '00000000-AAAA-AAAA-AAAA-AAAAAAAAS000', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAB026_Tổng quan.jpg', N'Tổng quan', N'Overview'),
    ('00000000-AAAA-AAAA-AAAA-AAAAAAAAB017', '00000000-AAAA-AAAA-AAAA-AAAAAAAAS000', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAB017_Sinh cảnh.jpg', N'Sinh cảnh', N'habitat'),
    ('00000000-AAAA-AAAA-AAAA-AAAAAAAAB018', '00000000-AAAA-AAAA-AAAA-AAAAAAAAS000', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAB018_Bộ rễ.jpg', N'Bộ rễ', N'Tree roots'),

    ('00000000-AAAA-AAAA-AAAA-AAAAAAAAB027', '00000000-AAAA-AAAA-AAAA-AAAAAAAAS001', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAB027_Tổng quan.jpg', N'Tổng quan', N'Overview'),
    ('00000000-AAAA-AAAA-AAAA-AAAAAAAAB021', '00000000-AAAA-AAAA-AAAA-AAAAAAAAS001', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAB021_Trụ mầm và hoa.jpg', N'Trụ mầm và hoa', N'Cotyledons and flowers'),
    ('00000000-AAAA-AAAA-AAAA-AAAAAAAAB022', '00000000-AAAA-AAAA-AAAA-AAAAAAAAS001', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAB022_trụ mầm và hoa.jpg', N'Trụ mầm và hoa', N'Cotyledons and flowers'),
    ('00000000-AAAA-AAAA-AAAA-AAAAAAAAB023', '00000000-AAAA-AAAA-AAAA-AAAAAAAAS001', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAB023_Rễ cây.jpg', N'Rễ cây', N'Tree roots'),
    ('00000000-AAAA-AAAA-AAAA-AAAAAAAAB024', '00000000-AAAA-AAAA-AAAA-AAAAAAAAS001', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAB024_Cây non và tái sinh.jpg', N'Cây non tái sinh', N'Sapling regeneration')

insert into tblDistributiton
values
('00000000-AAAA-AAAA-AAAA-AAAAAAAAM000', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAM000_Phường Ninh Giang, Thị xã Ninh Hoà, Tỉnh Khánh Hoà.png', N'Phường Ninh Giang, Thị xã Ninh Hoà, Tỉnh Khánh Hoà', N'Ninh Giang Ward, Ninh Hoa Town, Khanh Hoa Province'),
('00000000-AAAA-AAAA-AAAA-AAAAAAAAM001', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAM001_Phường Ninh Hà, Thị xã Ninh Hoà, Tỉnh Khánh Hoà.png', N'Phường Ninh Hà, Thị xã Ninh Hoà, Tỉnh Khánh Hoà', N'Ninh Hà Ward, Ninh Hoa Town, Khanh Hoa Province'),
('00000000-AAAA-AAAA-AAAA-AAAAAAAAM002', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAM002_Phường Ninh Hải, Thị xã Ninh Hoà, Tỉnh Khánh Hoà.png', N'Phường Ninh Hải, Thị xã Ninh Hoà, Tỉnh Khánh Hoà', N'Ninh Hải Ward, Ninh Hoa Town, Khanh Hoa Province'),
('00000000-AAAA-AAAA-AAAA-AAAAAAAAM003', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAM003_Xã Ninh Ích, Thị xã Ninh Hoà, Tỉnh Khánh Hoà.png', N'Xã Ninh Ích, Thị xã Ninh Hoà, Tỉnh Khánh Hoà', N'Ninh Ích Communne, Ninh Hoa Town, Khanh Hoa Province'),
('00000000-AAAA-AAAA-AAAA-AAAAAAAAM004', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAM004_Xã Ninh Lộc, Thị xã Ninh Hoà, Tỉnh Khánh Hoà.png', N'Xã Ninh Lộc, Thị xã Ninh Hoà, Tỉnh Khánh Hoà', N'Ninh Lộc Communne, Ninh Hoa Town, Khanh Hoa Province'),
('00000000-AAAA-AAAA-AAAA-AAAAAAAAM005', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAM005_Xã Ninh Phú, Thị xã Ninh Hoà, Tỉnh Khánh Hoà.png', N'Xã Ninh Phú, Thị xã Ninh Hoà, Tỉnh Khánh Hoà', N'Ninh Phú Communne, Ninh Hoa Town, Khanh Hoa Province'),
('00000000-AAAA-AAAA-AAAA-AAAAAAAAM006', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAM006_Xã Ninh Phước, Thị xã Ninh Hoà, Tỉnh Khánh Hoà.png', N'Xã Ninh Phước, Thị xã Ninh Hoà, Tỉnh Khánh Hoà', N'Ninh Phước Communne, Ninh Hoa Town, Khanh Hoa Province'),
('00000000-AAAA-AAAA-AAAA-AAAAAAAAM007', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAM007_Xã Ninh Vân, Thị xã Ninh Hoà, Tỉnh Khánh Hoà.png', N'Xã Ninh Vân, Thị xã Ninh Hoà, Tỉnh Khánh Hoà', N'Ninh Vân Communne, Ninh Hoa Town, Khanh Hoa Province'),

('00000000-AAAA-AAAA-AAAA-AAAAAAAAM008', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAM007_Xã Ninh Vân, Thị xã Ninh Hoà, Tỉnh Khánh Hoà.png', N'Xã Ninh Vân, Thị xã Ninh Hoà, Tỉnh Khánh Hoà', N'Ninh Vân Communne, Ninh Hoa Town, Khanh Hoa Province'),
('00000000-AAAA-AAAA-AAAA-AAAAAAAAM009', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAM007_Xã Ninh Vân, Thị xã Ninh Hoà, Tỉnh Khánh Hoà.png', N'Xã Ninh Vân, Thị xã Ninh Hoà, Tỉnh Khánh Hoà', N'Ninh Vân Communne, Ninh Hoa Town, Khanh Hoa Province'),
('00000000-AAAA-AAAA-AAAA-AAAAAAAAM010', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAM007_Xã Ninh Vân, Thị xã Ninh Hoà, Tỉnh Khánh Hoà.png', N'Xã Ninh Vân, Thị xã Ninh Hoà, Tỉnh Khánh Hoà', N'Ninh Vân Communne, Ninh Hoa Town, Khanh Hoa Province'),
('00000000-AAAA-AAAA-AAAA-AAAAAAAAM011', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAM007_Xã Ninh Vân, Thị xã Ninh Hoà, Tỉnh Khánh Hoà.png', N'Xã Ninh Vân, Thị xã Ninh Hoà, Tỉnh Khánh Hoà', N'Ninh Vân Communne, Ninh Hoa Town, Khanh Hoa Province'),
('00000000-AAAA-AAAA-AAAA-AAAAAAAAM012', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAM007_Xã Ninh Vân, Thị xã Ninh Hoà, Tỉnh Khánh Hoà.png', N'Xã Ninh Vân, Thị xã Ninh Hoà, Tỉnh Khánh Hoà', N'Ninh Vân Communne, Ninh Hoa Town, Khanh Hoa Province'),
('00000000-AAAA-AAAA-AAAA-AAAAAAAAM013', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAM007_Xã Ninh Vân, Thị xã Ninh Hoà, Tỉnh Khánh Hoà.png', N'Xã Ninh Vân, Thị xã Ninh Hoà, Tỉnh Khánh Hoà', N'Ninh Vân Communne, Ninh Hoa Town, Khanh Hoa Province'),
('00000000-AAAA-AAAA-AAAA-AAAAAAAAM014', N'00000000-AAAA-AAAA-AAAA-AAAAAAAAM007_Xã Ninh Vân, Thị xã Ninh Hoà, Tỉnh Khánh Hoà.png', N'Xã Ninh Vân, Thị xã Ninh Hoà, Tỉnh Khánh Hoà', N'Ninh Vân Communne, Ninh Hoa Town, Khanh Hoa Province')
