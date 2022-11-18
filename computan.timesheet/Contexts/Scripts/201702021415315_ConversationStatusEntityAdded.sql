﻿CREATE TABLE [dbo].[ConversationStatus] (
    [id] [int] NOT NULL IDENTITY,
    [name] [nvarchar](255),
    [isactive] [bit] NOT NULL,
    [createdonutc] [datetime] NOT NULL,
    [updatedonutc] [datetime],
    [ipused] [nvarchar](20),
    [userid] [nvarchar](max),
    CONSTRAINT [PK_dbo.ConversationStatus] PRIMARY KEY ([id])
)
INSERT [dbo].[__MigrationHistory]([MigrationId], [ContextKey], [Model], [ProductVersion])
VALUES (N'201702021415315_ConversationStatusEntityAdded', N'computan.timesheet.Contexts.ApplicationDbContext',  0x1F8B0800000000000400ED7DDB72DD3892E0FB46CC3F9CD0D3CC448DE44B576D8D439E09B72DCF38DA72294A76F744BF28281E48E29A873C45F2B8AC9ED82FDB87FDA4FD85050890C4257123C1CB51331CA1F021800490C84C2412C8CCFFF77FFEEFF9BF7FDFA59B6FA828933C7B7DF2FCF4D9C9066571BE4DB2FBD72787EAEE5F7E3EF9F77FFB87FF717EB1DD7DDFFCB9A9F792D4C32DB3F2F5C94355ED5F9D9D95F103DA45E5E92E898BBCCCEFAAD338DF9D45DBFCECC5B367FF7AF6FCF919C2204E30ACCDE6FCD74356253B54FFC03FDFE6598CF6D5214A2FF32D4A4BF61D975CD750379FA21D2AF7518C5E9F60A8FB431565A70440F98050758A9B57E87B559E6CDEA4498487748DD2BB934D946579155578C0AFBE94E8BA2AF2ECFE7A8F3F44E9E7C73DC2F5EEA2B4446C22AFBAEAAE737AF682CCE9AC6BD8808A0F6595EF3C013E7FC990742637EF85EA931689188D1718DDD52399758DCAD7276FD30465D5C946EEEAD5DBB420D5403CC779814E69CB1F369AF21F5A22C1B444FEFDB0797B48AB43815E67E8501551FAC3E6EA709B26F19FD0E3E7FC2BCA5E678734E5478BC78BCB840FF8D35591EF51513DFE8AEED81C92EDC9E64C6C7726376C9B716DE8FC3E64D54F7F38D97CC29D47B7296A8981C3C5758527F41F28434554A1ED555455A8C06BF9618B6A742ABD4B7DEDA38254947B3437CAF0DFA6012659CC86279BCBE8FB4794DD570FAF4F5EFCF8E3C9E67DF21D6D9B2F6CD85FB204732D6E541507044CCBDC2B26A7240DD0ADB99768BB2D50591AFA79FEECD9B3001DC5F5EA987A09D04989A90479AE6E9C63C1573C7AB6FA5BB237AD4D88C9EC1FF2CC4877213AD9E5B7493A7A2FBFA3DB32A9423091B99FA48CE22AF9D676F4C73C4F519479F3DEA12402736BA2D7173F87608A0211319667872A6E3A7B87BF7C4E76FE02E3B0DF1A805910B7C75336CD37081910B41AB18AFFDBA79B4FD1B7E4BEDE1FA40EE9C64874114C176FF3344531DDBC7F45695DBF7C48F654373915EADE349BF1FB22DFFD9AA7322C567EF3392AEE11AEF6393754BACE0F45EC31EAAB7A936A46A01DEA8D584F1EA950DC8E411CA858A7998CEB38AF0FB7B465196A94303AE199F0A33C3FEBB429071D8BAD4F7F558B0158352EBBC615D708B3F738890AB6048D0B23CDB8E186D18442ED84EB0E35C50E156E4382E5BC66D7EA2742F38CD826EAD15E631971287BC95105CA9310A62F5F8C2A4C271183ABE8588EE8F0E0CAFA14DB8F15EBA64F82FF465666923237915110E6CB5F1ABA7819A00B8B10F9D98D197C15B82446F3747CD891D61C8D3CFFC9C9F2E2DD6A957753CABB02D53C1BA56F3156EEF39EA24F81B24AC1C15A88EBBA7B72C4AA961C359B7E44DF503A8C476B102B834EC6A0E64EEAD5F874D8DDA2C276005A19FB093376BFE37FD77C656807236A813286AF0A7739D49A1AB7D84F09130F05B72FF2FF452E563CEF5DDB41C44CF71A3A8E4361B2DE86917A84EBE6D17FF65159FE9E172138DEB7E7BCA846472C16023B7A953472470F7939FE6C3254E1B5FABAC55231C9C6C7DDBAF38D6BA4070E9B90C1BEDBD46EA0269CE9DE585335E29BABFBDEDB2A4AB9F35C587DDB44EA6A8EB3A075FB4F811081C70C6875DB04482DC7F1D7557D877F45774CEBB8DB7AF08059B171A44D9D61773E12FE86287CA4DEAAF32DE410B79EBD96B3033973632B15FCD990355DF96FA4A7C28B7AEDE2D9EB16957191ECE903B991E54E59454545B8DC97BF09DDA6888CB14FEB55DC2D47DCF57C15D3E833EA7B18B144518AA4626F95AD160726C5ADE940AAA90E50A8A01DA758ABC7DB4CD6147E9CD973B0F2034DF39406EA9AD75F9374A87991C258F73BFB7E57124C05B22C46490848AB881D51C4D68C61398052E6B96155A133285FC3700C15AA0D3A89B2B1F88B84BAE12A07267B63B85E021EADD87066C6F76974DFFF0970D7FA49B0E5FAF477E5C259B8F0F2F1E23B71D527BB781F3EE4DB3F094E1C7983440DB6600B842FABE4595510CFC422BCF2277515C593D8765AFCDC60022AA3FBF1ADD85D8F65A3824ED62126FDF82BF93365AF55AD425B5CAA03DD10D82468B87715F389D086F5C9B9C524422FDBE81E6FCAFD272C1A9B86A714E4FB02832317FFA73CC41F36CEED3A31FBC255CCBE7C7E7BF7F2E71F7F8AB62F7FFA037AF9631F91FBA187C8FD10C23DDE53587EB229413F05E9557B68FE42E2028087667EBD6F58B5EEC0AC962A8765A0CAA08372038F800A4FD60DD4E5933619A94ADE605532A13E9CD07431353734E31DB75F77D34C55EB433D4C33A4E1AA790E3E03FE611407AAE8F6B640DF92C8A23B86707ED385E2594F9F8B321DB7BEA6C03E58F3F24D5BA3DB02850265F7134B076D7C9F51B4EB238548BB5508ADF6E15536043B561196BA44D443A81F43D2D62B5BDAD9B2C2D81AEC40112CECD9CA958BDAB1E99E086CD71D8FDDD03ADD862D15295BB65CEEFB42859C9A6C63A275C0319122D398EAF2418AC49BFD1E0B8E7A74741CFE224C02B10439762CA69EF7495156167B8F6BA82CCFAE3F4673F5FCC61A8D358CB5F5ED14C1586B9DFE83BC255906464F009EADFE3A7E30D6AB2982B15E4E128CF51D2A93FBCC769E0F4303F8E71D9ED2879DF91A28CCEDF5C710CEAC575810E799E7B3EF0B6BE44037C3B4432F6FF3EC2E29769D92D2F72474C5FC49FF332A1F461738D7283E1458E86099B033F16A98DE6A661543134CD157B0A5F9FC7BFE1E1F77F3E22223AD06C3FB98C75FF34375916D891EFCC55F2D6E010419CE9B38C69BDC7B4CCC685B0B7CDB2B1A3338A259CD7D31F4368D921D7C3324E980374DD54EAB856B28CAADA69AAFDEFD31BF4F32B7A13655F543A535AC4365D57C874A80B98D94D5D40FB4AE601D27AD15ECDEAD5EA1F0176F35D8E5DFBC0D3B698CFFA26EAE6BBB7AF948A7E3ABF9A4A73F47E9217457BDB8A11602E1B9A106BB7C6EA887893F7F4BB6442B71B88E6E2A63F04EF5E19B6E3BCF49239B9A1D84694EDDF93432C0CB60FE174CA34427C3CBD2D76ACE815882C969F1A6F324FE8A063BED12033C912F55547E1D0C0B2F55B4DDA26D92ED92EC3041549CA4ACFB2B0FB56E7E8787FB3854CD5F8DFA535EB5D534DC4B60D42D5739E110858D8B596F5CE930E6AC4396FC76402589FC568C2F00AA7C9FC4A3F7924665B5456982F1F85825DD99BDB75C2893BFA161B2963D528F871B221E224C1B55143F0881CCFA8ACF6447E2AE4559F7B0BC2FA4BB34BA2F6B472BBBC3927DF976F936B94BD036CCF2051916DDC0AB472032A24BF76C6FB97D9C2AED97D06D6785EFEF43B2EEB4A35E9FF37E8ED085754D7E377C2DEECA5A2E542FAD951ADEC11EEA665FE8C2FDF1517FB34E7B02AB2B23066AE9860E55F59D030565C7B2584F19355FAC1BAE50A7DF38A9F9C6364A5A4B334652681E615D63D85B4801597D75C3D5497775D25D778D10E7332A10FAF221A9F324B8F0382227AEECB47476FA50A17E4FFDDBD62B3B4D651EAD936BB2B3F618CABBD4DD6D8CF9314EF6C92431C56FF3ADE9F22254847447F3D308DD655BF47DD21EA7314711214BF89EC9EEC162BB8187490F610D2D1CC01275C6A9FEC092729F468FF10478A53D55A66C79617ABAC387ABD13B19DBB0D7336F4E56A07D3A0592938CEC18A862E2FB01459398A4A56E279079BC7D33901639B1417547A419169F9CBC186DF053911F167E65824FBCDCA3F57E1C235D16F48170B8A5815247BF8D99507BA1174D93E830FB5EA982E0D0A4E636BF1DF28AF214BB4347E5C0A50F7255B1DE38AC87DDAE9B5E0942BAD32B941F442DD51899852ADE370EDAE8B11C70256EAC5C661A993E566CB89B10DA91CB6D88AEA67106C16E456C53686A81A36EEE68F4036D2E1D7A8D8D4078D32AC7969176156F780B0E346EB0AA127FDC561F8A44EE36A7FAF99D7132B8867516621DC3F0A58AFDC66DBD3DE3965B7783A654B153CEB09BB4E6A9A376D0AC8611D76A1D0DAE818A83A2D5C34C30C424D9C1598D9376E364772E9FE078C82CA17891068728A247B469CC78A4A7348F6D1E9F41FBABA6F06F98E488BE8FAA11FC231D76A0B1F752BD5C37EFBD03E564DF77ED3C8055323A5FDB04105651491CC6A73CAE363D56F9D43D06381BFF7D1B0A860BB61E7AB55E946994C211F4EAAE27936A2DD7721A7A4F05FB603FFD923E0EDA136F536A19E46170A81DE928D0FB81CCEAF334FDEE40FC9EACF6B130C2693AEB719DBC3B44787F8C6263449740F7AD13A66E9C2C517BCDBB7B42CE610CF8B709AD6EBA1358ADDE3E56EF9E5C3DC0EADD986FB4866FA98266E7926B85367F37F0351670A1D832C47E7670E2056C317EB796B0B6AAD6A8C66A5846DA56EB6F480E6F05B40CB9BF66E8885D33661DB03A3C8E2181D03F9566D77A55AB264AA2B9A05C90DDEAABB24C2E530859A9E043C66FCA328F69D479364A9AB8F76D9E555197FF579CE145B6DDB0DC9340E56E8014B175B24BBE1E4632A6DB84C404C2237A7DF2CF0A0A2D1DB4770E720732E4E70A644CE3A8A0D9323150925628C92A9521922C4EF651EA3008A9AD233B9105697B914BDEA13DF146CE2A0724BB74DF654B5707D1F62571BB0D4FE7671CE5B81014CB5DEC444F425D3D39F9D39108D8998C9E9D9E06A5247018931112885D97DEF775BBF9E8A8CD735BDE74FF7F8B3790FBBC78D49394B119485D5D0B2F12337704519B3A8B910498D3D0A6A040A7C570126A6DEB98B55E1861D661523DA9B26E333A49D25E8CF448073F25310A839A8D128505F023C3B4098BBB201AAC1D21FD8843F4971E890205976BB013EAC23921FDF1439A8DFC78E43B525FC69A929700B3515F63E0B16877623588CA5A83933B854940A73E1BC0DD4F404130328FE03CD00C9CAAA1CD8ADB9657A81D987444D800056960873B17184732213181583EB2B3818DA280BA236D78735294611C136F6F7DA8A9BB819C999C04AB9C7DCD61431D44548DA9B80F6581D63EAE1766431C597B824631296941C87619406B3D9E85B68014C6BA1537E533EE565BCCAA6D5B7423748096D404C7368AED4556FAA9BAAC289879D89DA4F49870E91CCE673C1135898940754BADC90ACA490C9A54D85D18C199447949D7E4261D450A81DD4F207C40443AE9DC5DCEDD59E844CE40A75B576D3ABA6E65F97493EE04A3CB6327011E895E34BD4F40311A84BAF4DCE49A9C856034F95474CB6B4BAED2ADB292EFCF7DC7B2A466D1EC5A2C3DC828DB9679DA136C5D6694B80C00CE67301395B15438AE0420E7C51985CAA4AC3A1A2A63693726A13271DA3350998892A3A3329AC6C875FDA59C46A3D0989811697AF5DB38E719084CC0C7E2E94B4EA5EBA003894FD2022A57C243B621C43A54CFE2073201056970EBD273EBF4330FF12861ADB5ABAC8F71CD11100B52E0413CDAC0D81C583EEAF63894A31BC5143ABA0EB12E7D8B61FFE7242128C48665D18DB136021095293887BB700A6715B70F6B0A5165C5BD931553E3B93827050A8E7916D280DDF302D01CE8CCA7009E42984123994E9C41087627AC659092F1018B52730432D2BD54E18637B6B0520732350DF93E4BA15E8FB3BE48E15C4E6CF7C16A553D1D5187185F5A0263804D7919AC1FC664A404E1D7A5F3B9AF82792727E325B05C711422F2BBF21D8580A6BEEFD5E1D5692B9BF5A6971FB9B76A6E0F851792AC16A9A6DB863699AA6E5B8B2355D7F9A0860E44D21C5B46A0BF46DBD0E959A3AAE9C008A6156B2262DD15AC05900F1C1DCBBEE29650591069F181FFFA109925D2A50B3507273BE398262542E3827890248B743233594A1EEC76E2D0B9B3435451C7CEE94381BAC899B3901E3C9849690E46FA11139B93E1CB1C9E6A24825B8035CC34A099C8EE486D63424431372A38584F0E8388ECB09C03833C9C490F0932AA5D3A0782302EE34CE02CCD3C44D9A0F3C14284D8FC12ECB8C5971C5CCABCFEDA48538A2DBE096BE84B61BA2855D31B683523998CB634B83E1E33AD1810CC6DD92DC6DAC16435AFC9161CC5E4047594865B25729BDB7AAB61DC82939412026E3E954B37A4C9D42E1DDA9D8E915C4CD525109AABC5C214802F3CB12DC162A11FCCE4B2ECC82D1642C444370A18599A2D4C92CD2AC5FA3C5D9D8DA094A885BAF5D68730EC569B0F9FE94E49DAD887137943EBFA9F402AE990BA34F5EAA2F65FA8A31726192A54978C77B7A4107D87123AE149B2D0A9258BB6295302017E8D2A21D6E0C9867E0762C0289404B56FE3546AC0B4E536685CA6EDC638A182042A59E132DF5A00182BB1419063AF25081C1B10A0CE19721D4BCB0C95851973066986E6803821D292191A7DDB6701D81A0614406D89F3901AE967185353C50293493C0510FB6E69CD3FF55740F0851638978F17DF635447CB872089C51658D4634B0522C63EB000917DA94C003B7F2B1BAEA9E7BC8A6BFADDD29ABA3C2B8DE96787B68DAB1008A129B4C0A1312854108AFEE5815EE62D6CC62F73CCF500CBDC43CD609927A603F6FE92175F5BF51544A150C306913D555101B102A7F65ACE138B9D60E9845C57EA08889EEF347068A1331CFE1D8701225FCD19B66621C57237D4E9C9422C77602EAD60E70B25389C32A5D14E9A607B1BAE2AA4A58041F90425DA18C4BB9D90A21C29BAA30562A39A2B1095C99F89B377C68C18635A8B1843286A601670306AEB24AC9046458739543284198FE0CAE2D4DCC22BF3B3D42B6C3E9021FC815AED48D864BAAB2B2A8170C00EB31503020743A218011804DB68EF23618F6E3EAEC853DD801C2629780405439DE010044265BBEE60C4494155016499C2AE0A53D1045EE586AF39AE98A08C29C0E018A10614188289827380C389F644081C3FD40E6C203719D0628D87A9A574034AFA30CEC4481193A898F0A2375C6AE6021A3041ECC02A9D0B58004BF099DD1F4740D841003FB6E084C2240CE109B909B093B601198640841C1CE0F43E182962F43C001F86F07AC214E0007BFC3242E600030848B86A6C7BFED39683C1011337C68B132F733411E3B891EB8D114648000E40C3883F0234C1CD003CB884411326610984C6CD8531A101219678671AE66826131C4B8DF5C58E25288C9771665220AF415892E27569B0D44C263896986CB2238956749F97109369188A84F04A630B59393A9259DAA8F7D23A19215C4B0F9236C20DB40DB17D30401DBC78DB398003A59261EC725D100FA0D9D10207C082C1E4DF1B15903FAC1E2756EF59685226DFD95E583239C98E4734A2D9578B22FD936D682AE083ED5E48015F662B9042930F6FC0B6A0C4623A50EA864087CE462058E503E1420871A145863E1006300B301486320DC8EC6F0336EA29907F98A53BFF29759C86AF3DF3F96362F4731EDF978F90750B53A09D968FB0F5C2D71C4297778C37238C55729A4AB38D84404B236E743226282A60276F23621CFCC23553337B868348D35F157AF5A1452678C13904A172826D13264D0F8335D3D33C0D06E705DC61BA419D035B362DC8EE8D6B9D9B4D27EA8BB3893524C145D48A2FD591D4309F8355B6FBE1E730B54877A1227F12F2A01F3F213F31E5C8DE795A3419DDF88029E91CF954FD187E5AE1007202F552F430B322C74DC9845DD1022066226D53F194B2E205F6A932CC44F1AA0A801DC58D6A34E90378F8D851E4BEE91BFC8142A069EA4D5F7051B1E2C98B8C4293D0D8E4A3B85800E830BB610883D73A62484377B8C1D47A5E0C9032E767B471EB2CD0969D9F5DC70F6817B10FE767B80A793C7C88D2CB7C8BD2B229B88CF6FB24BB2FBB96ECCBE67A1FC5E4E6F55FAE4F36DF776956BE3E79A8AAFDABB3B3B2065D9EEE92B8C8CBFCAE3A8DF3DD59B4CDCF5E3C7BF6AF67CF9F9FED288CB358D00564D786B6A72A2FA27B2495920BBC2D7A9F1465F52EAAA2DBA8C4787FBBDD29D540D70811852D8E9B2E05EF0775D19A77874D75F27FDA04CF747FA8A2ECB4C2145D3E20549DB22ECB53185C87D2F77896E410554F18A9CF409486B8E9751CA551D178A4709E306FF3F4B0CBF49E31FAD65DEA511E863E21A91E5286FF8A50E8177708988E925404C13EB9C388B6DB0295A508A5FDE80E87383A8940E8177708245C049211DB7EF4184997FC4C188E3E279A1ED6DF92BD08A5FEE0412D0F79262D32FBE40E6397DF26A904A4F9E60EE577745B269504A6FDE80E2729A3B84ABE4980BAAFEE90DA00343C206D541AC37A178898EBF2EC50C5D2920B251E23A3064000A258E281B53D9E982C77D8373F8C410883F1757E26494E59529F29A25ADA3A65C1EFB12D34EFB6C3EE0E1AA8CE9B84B6FD387B4597FA5CA04A6D42743DA465EC15189BB220629FE6901F2BDF2F8BEF55CFD260CC6F05ED22011C808C23068633EFCA344F9669746F3DFB720A08CF893D342DC7E189A4CC6562CE3DDBBF5400BC9C962BB324460094F6AB07A4C32EC6C7750950F3D1F3B8A142E23EAF92E2582505E46D164C6C587DF05C2488039075835DD96626B6612E86E15906F4B5F4E2170D84A5324B3DDC4F07FAE699072414ACCCB7321FE7C2179EEF7A1D024DAD4732021534C215EEB3C9512711A65CEC059BB54D09EB01B0E5620F5DB28BB12BE892FAD0BB2EE38C996AA01F2A5FC3831F0AC9B8557FF0E3275532765F7D2E88CAF2F7BC502E889AAF1E90F2A292A0D45F7CAE4276840DA4DB9DEEAB3BA487BC94C642BF78EC3BA8C208F8BACD77110998236C4062D12AEF8F5EDE53AF83F0321FF2AFF012FB3080A56A5AAB66F4443945FF8CB11F8768E039B086B6E5383C11EEF9C4922ED7B6A88C8BA48E2E2802120ABC9E4214D53692AFEBB9CF5E3AC03E4564042A40B96C954C7FF79249092C127E1387017B6DE33A10E308AD3680B1C08CBAA8C67A38EC901325BA931A2B5999E7489947F722B61FC780D01CD844D36E5572576E98941B8CC125FAB1841EA4035F981AAFCCB132C7A4CC2104240FC51E26A00E0C626E3E0E8BA0A647F5742215F91C79B2AAC8D354BE9FE2BF7BBC908FD5735DF3ADC73C6FF0AA95D1BD6EAE5D711FD825F3B0814137A5BD205745147F257FF4E0F92A7DFAA86AA71EF5993A5C63152B8058D185F9EA274F848407FEF2C4DC5C87E70F128EEB2941798FF4203E29FBEE27CDBE3BD332A991CF422F599B52A2FFB2E941E8F04E5AC8CBD77C735F3DD22744043094B98E7A7588CD60473D089ACB510F6EB7546D36BABD2DD037EA8C296DA842C93CCE5EAB86BCA8AD4C1309B61F8341C01CF80B6EB654F65A0F8B4F98155890D1900C018374640B5DE371988324CF952134DFFC167B75397D72ECF145133ABE1F67C899C8FCD9C30AC1F5E8534FCC4F79AE433EA8E71FEEB3C743DF0802D57D7587F4068A32F0C63FCAC05B25CAC05BCF2803B5C62CE3B9FDE83112AA61CA90B8CFEEB0FE2A4719F8AB5F94812B35CAC0956F94814B20CAC0A577948177A84CEE3340BF170A3C6656E47778081F768AC14C2CF17CBC2E4BB48FFECF63AF3067E6990CA8FBEA0EE942F5CCBEF0F5CCAE1B60097697143B59E8CB651E73648F56FF332A1FA4790A251EDC87E24341C46415ED249A978A3CA91F7247100A7AC1D36014AEE1DEC3E7DFF3F75827CF8B8B2CBA4D65E86AA90775E7F1D7FC505D64DB7758A27D917509A0B8076C60CC7299C7CE10C7780F788F49146D6BE129ED116AB13B6CB27DAAFB57F775310A0C9050630C7B204D813ACC20A881E1A3D1D019FA6D91612C8B75CF3490BBB069779F3D61FD394A0F1030F67D9104A64D97329CC06832DC6104A681A1974AB83AFEF62DD92AEE68629197D651B7F9137A54948EAE602CE29DD1C6C027200E696830C075B43618218C6472A8632F2A4687F6AB9FF18238B85451F9153262F0653EE3DBA168BB45DB24DB25D941F6F2018ABD2C89A46979A8B7DFBB439A4A9C0056580D28476A40D12616E8C9F22038174ED7341C87C1632E22907A812496792C7896FC764025F1EA2C2496948A3C583DDF271255B34FEE30D2A8ACB6F8948BA7F548D64C04A7967A3C8F4EFE263B2AD45FDC21B0372FB1AAFF8B25EE101F22BC764DDE056919E4320F91B0231E985126BF81E1BFBB43BB4BA3FBB27E8F28539F58E2B7C6BB7C9BDC25680BAFB158EAE591028CB3CF18E9060AB9428B25BE636362FEF61134F2EBEAF4EC45366F2985EB5678D45B61E067CB26A0CEDBE2FA7479651109C69C2C12D6C55B9F3ECF993F8ECBB57BA5E905D2B426FFC5109286403A9334DC78E9E68E3AAE3053DC657872993BD4DB1833469CEC1355A1978A3C60E65BC9AE41BFCC7D86145AE2D3E27703605ADC0F3670AE048ADD611331433881893011B252E80F17AF32C2BA83067057EA0F191FCA2B182A2DF1809894FB347A8C25BC729FBD61553908ABF28A987A57E43BE994597F795A67EA242BD03E5530C67DF68155A122431513570F28524D3ABA3ABD7B5184B85ADCCF1EA0AA2C6AE9122C0D3B2222C81EAA9882F8027778203DF4A0062C07CA049F5C94174842C19456B3F2704BB3CD09409A8F3ED645DDC62A96F8DA40D5DD95FFEE0E6D1F30125DA8500EBF1DF28AD236BB5E4112EAC00A73D8DB56EBD87A4C1AE198C4A5410E7F60D203F73A3A99C08C7388EA7420198E58E27D2CC3B3D11CCD5889D701A00206C87DF68695E631F0E65329F4865B294F75848229ED35FBA892DE40D22F0B64CDA0AF374C50BD9871FA971B0179262AC9F365EDFE0914FBC3AE72236CAED81FB6E26EC97D5F95121D1EE6B5AF877E876582EA6E649FE50D56404E26EFAD20558BFF7EDC47933A94B2124CA5FBEA83FDE1F9DE4287C60C17D2BAE6913D5179E0131C54EE615AC6EB461EC31B8E889A2AEBE1E8480F475FBA5CD6A184B61EA483C436351E475C8712610B5A6E254BB85CA5ED9D7D697FB759C259866E2175783D739208BC9E71C9B285CB29BB6995934DF3EEFCF5C9F56389B7BC5352E1F4FAB794E5BE6E2B5C46597287CAEA73FE1565AF4F5E3C7BFEE264F3264DA292A66F67C9C85FC587B2CA775196E5154BEEEE909DFCF94B929D1C6D77677273FF1CE7044A596E8530951C97B43E166A6AEFF33FA14779811BF2F915DD6D74747B7E26373C07689F744E24F37D42D05A73D47F20BCEA44F65DE1333C2AB2CE5DE164F3E94045F8EB93BB282D958D51EEA18B732DF623E3F3D50772ABF7FAE4BFEB76AF361FFEEBA669FAC3E697022FF4ABCDB3CDFFE6FBAF0AD5E144EE9E9E7969D7D9B7A8881FA2E26473197DFF88B2FBEA01D3CB8F3F7ACF8965917587EA32D23601B901ECF367CF9EF9C2A539C9CD407D61B629CAC535F51C5817B06608983A3DB96929BC67C7D2950785D9642F0F0AB4CD651E9612BBF76ACDBA54DE2CD21E5D4D84F7E2676F9A11943C0ABBB9B5F61FA3A0DFE98139E18CE97741D7B7D9FF45A0FFB88BBEFF931914BFFD3BEC36608EF063DE74BA9408DE9B4ED354B7E938F5EFBBEBB810C3289B0E3BDB86DD1D42C88F95D10333BA2515F814DC3E0AAB8FC16A2B012F91808164DDC7BC47D5D9BF4D8BE04FB5241BB801E2CBE087A79F9FF9F36B9B2B3C30DC267538055BEEA234ED719AE0F2860F03B4F27F60FEB7A4DA3E665100F183823B47A1B2EE5CCBA55C20E9F593275B973510926873BAE24AFC4F88F86738708C665E501358FBDB196418830C0E40DAEB3E2312610C1A11F71EC0DFF0DFB41D62F987D36B0FC04A07661062EA2CDD01846697A83B84E2D025EB0E02AD4EDA1D6092DD238700C068FAEE0080A4E4DD21E6B96E3B236D3BAA8BF431EF3CC154AE55575A1ED15E4179A98F995A67BEFB3FB65B2027A0C21BC20082804BB02D32DD0BF2C406C54959BF95F99F3D766E21CFF6408E5EC5D5E2C4953935F531CBADF6F99EB7DC602D439CE558626CDDD39895E467207920CDF431D3F9281BD42AAA9747B7BA34D0EBADFF4AB88B265C7D7AE66396BB5296E78164C2A7780E702068F23C07000564780E0AB549EE1C16289FD23928643E937308130EC8D2BD6DC9E332B23E2FB21B236B3222DBB9B96D68907CD2436427AEFB04EE40B62302190DF95FFDF987CD87F24B1D1EE6D5E633462D392B085BD84F965179631F4E6FECB602BAEC0D9AC5EAB3564D17BE58C5072FDA543C770D5CE166163D46439B0E188DFB8944CD867CCC3BA355AFFB83FFCB3031CF72C817775A0F16170B63D376A8AD60D537036E536AF2E327CD4DEBF9FEE9902D949CF89889B74975EC2D5969C3616F3474DE6B0EDDB3B6415591955FC2F28B315BB1BB3ADCEB40D2360C7C20E1D21D9B203FF357A0BAECC78101BF815D9E7B1D61DF8EE0E6DCE648D65DC2380DAC4B8F3C04CC5F83BB395F8DE0E67C39869BB3904839EC0A5F092995C3DA933FC22F313D79E4AA4DAF3C847A2E1C9C477FEA0594CB06DC5F1F14732B0790065266E5001085CCCA61E10541A19A42B93F2C2067F2303541CE95DC7F68407264EE9AC913589716D957CF6B5ACE68C703B212BB6B2EBA7CC46EEA4BD77AC42BBE651903B97CC92194152E61F2F836774D8A61376AB16414065A5C9932063B1B96ED94288D2CB03A2DCC2230EC3949DBCBA6A04D417CD4868536BFCF20C54CCE623C0C989AB938C40D2194B178B5DA2DC50A01A5FE3D66C692933FF98A37B17D2BE39E5BF52BEFA39994953800ABB1BC510120A9698907721ACDB132483C892989FB2BDC72CAA4FEB2884F95D41F8A9874989B97C34ECCB71D646C5633070D5DF09E330A321B31A3B1BFF19E6B3EC4F7431749BD87CA05837255C1BC07DB19B9FABF085B37EA1136EAF501ED7A337CA4A45B1B2E9E8C9E09D16E3F3FF595DAC6A0363585EF31539BCE5CE0ACCC0C54A8E4F4C1018E1852F2E01010EBFC86010099CF90C341D2D4C181A1863BFC29098307CA24354F7020803445F050605D82E010B8EB52040780463305070034E699B75794AC2E2F700813A326197078D081A4819A013830D020477821F16F8001865C7221F96F7F3AE4CD52BDDA37597F8358EF02EF877CD6DF00E0F67307D31ACD23DEA57330AFF000C299D33CB6DAA5D6E3D608C72D5DDADF633E7889E983434879217F62DFD35CDD7C98D34E977638CC41444C391C0E6615EABD4B30458B26209E82A39EE29387D9291F481AECFEB4C6C9795DCD1CDC636755A084DC54F93CC4C77CD1E7AAC9CCA982F89AC99FE853A7D9F99ECF34DC831FBBE62119F1EFFB50D525471E2884C01452FD0C7881431B060D1E0CA546EE7F06D5E4410601AEF183DD4E56CEDE9943C488F38EA2CB447CCCDBC9BC7111839FA7DF94651ED3B810AC0B211DE20D988AF722DBD6515CDA4CBD6C3C24D5F069F3E9F29056097131C5BD62A252662603691230AAB0DA1211E43F2B2031EDA08225C8C93312C42851B3425F15491627FB2815A7205573A44882D816A05CF20EED697E03709E2E1D76D17BD56E5BE8128BD890707EC6ADB90B29DC5CD571884351C2B3D3533B31D0A4D55658474A02F5EC5C7AEB8247CFB3FE5DE2921B6BEE296E09D5AAC25202C51EC2824BA602432DC7A2120B0646A2185DF2184D7770CA8F05910F94000A5A5E5A0F5E6256765C54034C7C6924D326CE5910BD00EFF6A075FDCCD969A5A5A545C7452CEAAC17432B72DAA75968E58A1A1F26D14F595F0294F6DB912B245750BE0DDDDACFAB8D364B4ED55170E0FD16CDA291B2568A4A3A2E09F82C4C181A3846AD743A2A986F539890127C7703CE7C3C33295033D30D949CA05B435ACAAF1EFBD2473760862D98169AC251080298E4D8E40059F1347DB606B25928828FB15C070A904D8FDD520AE198F965140BDC694389300C01ED0A47A10D7D8C69CD7269824A7BD18939B4B2A163B8DF09C8A48E6A76C3C292E9B547562EF078F3CD9D2E687C6241ECD02FE3E88ED0A4346B30503E008197351D7151826759EF2E38E80D10DEB65BAABA905F29FAC17DADB928A43298E6F328ABAECE6A9C25D74559D5F4D604309D65D1A508973734649376EDE58098FCFA2965FD76051AAF4AB72DB0D251E8C318ED53B3789AF09EBDB706205A97A1E7D9F606996CEA184BB3920D0D5CA5231B56FA64C90608DB750C64532B567352CD5C2AE81268C6590B9D956438AD0408C13C3AB1CCA5ABCC45209EBA4BFB9A731EDAA8DF1DDE68B3D675CBC855E19791FFEC4113341E97400FECD328B4A09B9F664D866AAF40B4314D4F6258A4394980A2E70B7D90F6C7C7B12485C5FE393561CC2624DC4944F7FA7B4E623184E991D712901962C172A5867E969AA59A4E6E2C43666843DEC86BA65C9BF39F172D2074539C9D02C4E86F33520171E19AF0868C0B7CA31201FD7CECF763BAD83E9ADEE6BE1EE3C820C8C5D8420960B2FB30CFE59FF7268C5FFC056990D393C7BC9AA43BB52C4F9BAC89078CD0DC63DB5F226978ECEAD38A8E2EE0DACCCBDF0555B8D106B61BB08E6E44C1457680C1F215462414E7E50B462CBA981666B261BEAE3393CEC7FC7E469A213ECC30BCBAE4495189E2AF7D5CE4318FB5625E2299DA76E143268B3061342472985B599D8346E657595D69050836B20C85753EA132870A3B8738391E59C2629A4C6E0B6B62A900F6D5A6E46958C4C0A0319A0E9761146B48623ABBD85CC430B175CC87149660206B08813C0E98D53036177DCCAB6BF8900B1F4169091433CBD9762E3299E36CEB451C4B39DB36E4B10A93450B935905491718697C8F352E08130F87FF7CDC7A872ECA94A6B729958E8BFA416A1DF127C950D1782DE65BF43E29CAEA5D5445B751A9BED120ADAE512584E139D95CB4119C2457F6EBF801EDA2D727DB5B12DD9E8680A26525401810F036C492A68FB65CDF15ABE2D0239735A43967ABDD0295C0BE817AD6FE990717D0292B817B22850972E8000870A3F6055402BB95EBF98D80454A3174CF6A98FBAE2BF9F40BAE295F6AEECFA72BFA56C9D01BAD60EE90D4B177DA1A0394DEDA12A81B56E833A946341BA6D554314FACA965E9996D2B4A77EC3BD4872364FE1DB2029E2F84FAE0CB2DFD5C3E5E7C8F511D3913EA492C86FA126B587A137D8495DEC462A837EA1EE2DA4DE7B2A1EDAAAB0275D7943A5002F54D5529817E07298114D921535F4A0530FD0CC125256E601B7F05107853A8EB8296DB3B521473A537A5866E29FC569E39061A979ED5D175C87C1F7D7A657E65C65E591D5DAFCC75CE6505FF92175FDBB309B88C420DDD5A72951CFA650F55D4EE5801D84B5DE60A5C2BF9C4627D478ED28F7FB6ABE94BBF0D76E5AE1D51DB85A6235AA8EF8894FB74C4BFEE3074C9573377FE86CF45E63C0C0D658AE5E68E3DA8D2C00B62B961415DF9803F0B2ADDF1853A3ED7A802DCA94873E068E2776DB8AAD0C1038CF3251C73C54312EEAEFBA41CF4E456D2E9876BDC9648333B13A7E63C6D3196AA76D68690ABC1264DE359595BF699AA396C28346B8F40A3E254B4472D3A25A0D88418F5F8228151693C147AD831CC1537603049531C4D703AAC6CA128A11B962B46204F1043A448702EB468767448C10D011498C21F069010D239B76ED67E0B363D31909F619686887F7D870DB5532462B8294331EB2C943DDE74A7E669C1D26D9CB5DE260E59C5B9A1B32F4E9316351D69E61A4DC67FFA40203260EAB67065C214209B433D7CB1C080049D394180D2150E468218660B98BF210E97B87AA2CD962E5AF3CD3061C1984129857E193C3539A214303963D029F1D28EB38CD4A3A41F0C13538D1E6DBBE6F3E0296AE22701337589B464BEFFE4E6A0943912B4602551289A9506474A63E2B023058A23343A5204238E8214561A1C29CCD268C709AD382D4AA6107C726417B37480AECA4740C2143283F99FF3767B60EA4A25FDA0D5EB837AD0FA8B01E03584385DF629D45421E74AFD9CADAE98E3ACFCF86810AD95DAF99B9E2B6B9F672BC35EC6CAF36655CB842D8753D57ACB0DD87A281D7FAA422401ED5CF5F106821C5B54CB333755FA39E474B5C715A54ED883CAD4D3F4115F6EDEE4238AB0C9D0D230901113B0FB701FCE9C71AAB087AF71E20E4EC1C326A469AD5E044970F80A015124BDF635E2C6F4327814A47097441280BA641434D8B679BB7FE728BBFDF4E83858C525ECC338B2889C0E091E043107358C2D4065CF322D028C2E6801D523E9DA9857239B92C053B7E8490647AB40AAD2F4536E7D85ACB386BD8A46E4FE1990E1B031DA5C66826F8CD3A3C18D1E9E1C2D289E0C000ACCDE0E83A580FA7AA56E687897A29F669326B97DB2DF969D9FD1C72FEC03FE59E545748F2EF32D4ACBFAEBF9D9AF878C2408A7BFDEA1DA35BE01718E6166C4F195731168EB7CC8EEF2C655411A515345CA047D89AA089FC0A2374595DC4531B91D8D515926195ED53F47E90157B9D8DDA2ED87EC9743B53F5478CA68779B0AD725C4E3C1D4FFF99932E6F35FE803D91053C0C34CC821F297EC8F8724DDB6E37EAF66C2D68120AE142CB93759CB8A24F9BE7F6C217DCA3347400C7DAD07C867B4DBA7E455EB2FD975F40DF5191B26C08FE83E8A1FF1F76FC996B0AC0E887D2144B49FBF4BA2FB22DA950C46D71EFFC434BCDD7DFFB7FF0FD1D8D4757E9E0200 , N'6.1.3-40302')

