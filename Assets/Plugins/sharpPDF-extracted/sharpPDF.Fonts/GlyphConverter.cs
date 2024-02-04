using System;
using System.Collections;

namespace sharpPDF.Fonts
{
	internal abstract class GlyphConverter
	{
		private static int[] _unicode;

		private static string[] _glyph;

		private static int[] _pdfcode;

		private static Hashtable _UniToGlyph;

		private static Hashtable _UniToPDFCode;

		private static Hashtable _GlyphToUni;

		private static Hashtable _GlyphToPDFCode;

		static GlyphConverter()
		{
			_unicode = new int[1051]
			{
				65, 198, 508, 63462, 193, 63457, 258, 194, 63458, 63177,
				63412, 196, 63460, 192, 63456, 913, 902, 256, 260, 197,
				506, 63461, 63329, 195, 63459, 66, 914, 63220, 63330, 67,
				262, 63178, 63221, 268, 199, 63463, 264, 266, 63416, 935,
				63222, 63331, 68, 270, 272, 8710, 916, 63179, 63180, 63181,
				63400, 63223, 63332, 69, 201, 63465, 276, 282, 202, 63466,
				203, 63467, 278, 200, 63464, 274, 330, 280, 917, 904,
				63333, 919, 905, 208, 63472, 8364, 70, 63334, 71, 915,
				286, 486, 284, 290, 288, 63182, 63328, 63335, 72, 9679,
				9642, 9643, 9633, 294, 292, 63336, 63183, 63224, 73, 306,
				205, 63469, 300, 206, 63470, 207, 63471, 304, 8465, 204,
				63468, 298, 302, 921, 938, 906, 63337, 296, 74, 308,
				63338, 75, 922, 310, 63339, 76, 63167, 313, 923, 317,
				315, 319, 321, 63225, 63340, 77, 63184, 63407, 63341, 924,
				78, 323, 327, 325, 63342, 209, 63473, 925, 79, 338,
				63226, 211, 63475, 334, 212, 63476, 214, 63478, 63227, 210,
				63474, 416, 336, 332, 8486, 937, 911, 927, 908, 216,
				510, 63480, 63343, 213, 63477, 80, 934, 928, 936, 63344,
				81, 63345, 82, 340, 344, 342, 8476, 929, 63228, 63346,
				83, 9484, 9492, 9488, 9496, 9532, 9516, 9524, 9500, 9508,
				9472, 9474, 9569, 9570, 9558, 9557, 9571, 9553, 9559, 9565,
				9564, 9563, 9566, 9567, 9562, 9556, 9577, 9574, 9568, 9552,
				9580, 9575, 9576, 9572, 9573, 9561, 9560, 9554, 9555, 9579,
				9578, 346, 352, 63229, 350, 63169, 348, 536, 931, 63347,
				84, 932, 358, 356, 354, 538, 920, 222, 63486, 63230,
				63348, 85, 218, 63482, 364, 219, 63483, 220, 63484, 217,
				63481, 431, 368, 362, 370, 933, 978, 939, 910, 366,
				63349, 360, 86, 63350, 87, 7810, 372, 7812, 7808, 63351,
				88, 926, 63352, 89, 221, 63485, 374, 376, 63487, 7922,
				63353, 90, 377, 381, 63231, 379, 918, 63354, 97, 225,
				259, 226, 180, 769, 228, 230, 509, 8213, 1040, 1041,
				1042, 1043, 1044, 1045, 1025, 1046, 1047, 1048, 1049, 1050,
				1051, 1052, 1053, 1054, 1055, 1056, 1057, 1058, 1059, 1060,
				1061, 1062, 1063, 1064, 1065, 1066, 1067, 1068, 1069, 1070,
				1071, 1168, 1026, 1027, 1028, 1029, 1030, 1031, 1032, 1033,
				1034, 1035, 1036, 1038, 63172, 63173, 1072, 1073, 1074, 1075,
				1076, 1077, 1105, 1078, 1079, 1080, 1081, 1082, 1083, 1084,
				1085, 1086, 1087, 1088, 1089, 1090, 1091, 1092, 1093, 1094,
				1095, 1096, 1097, 1098, 1099, 1100, 1101, 1102, 1103, 1169,
				1106, 1107, 1108, 1109, 1110, 1111, 1112, 1113, 1114, 1115,
				1116, 1118, 1039, 1122, 1138, 1140, 63174, 1119, 1123, 1139,
				1141, 63175, 63176, 1241, 8206, 8207, 8205, 1642, 1548, 1632,
				1633, 1634, 1635, 1636, 1637, 1638, 1639, 1640, 1641, 1563,
				1567, 1569, 1570, 1571, 1572, 1573, 1574, 1575, 1576, 1577,
				1578, 1579, 1580, 1581, 1582, 1583, 1584, 1585, 1586, 1587,
				1588, 1589, 1590, 1591, 1592, 1593, 1594, 1600, 1601, 1602,
				1603, 1604, 1605, 1606, 1608, 1609, 1610, 1611, 1612, 1613,
				1614, 1615, 1616, 1617, 1618, 1607, 1700, 1662, 1670, 1688,
				1711, 1657, 1672, 1681, 1722, 1746, 1749, 8362, 1470, 1475,
				1488, 1489, 1490, 1491, 1492, 1493, 1494, 1495, 1496, 1497,
				1498, 1499, 1500, 1501, 1502, 1503, 1504, 1505, 1506, 1507,
				1508, 1509, 1510, 1511, 1512, 1513, 1514, 64298, 64299, 64331,
				64287, 1520, 1521, 1522, 64309, 1460, 1461, 1462, 1467, 1464,
				1463, 1456, 1458, 1457, 1459, 1474, 1473, 1465, 1468, 1469,
				1471, 1472, 700, 8453, 8467, 8470, 8236, 8237, 8238, 8204,
				1645, 701, 224, 8501, 945, 940, 257, 38, 63270, 8736,
				9001, 9002, 903, 261, 8776, 229, 507, 8596, 8660, 8659,
				8656, 8658, 8657, 8595, 63719, 8592, 8594, 8593, 8597, 8616,
				63718, 94, 126, 42, 8727, 63209, 64, 227, 98, 92,
				124, 946, 9608, 63732, 123, 63731, 63730, 63729, 125, 63742,
				63741, 63740, 91, 63728, 63727, 63726, 93, 63739, 63738, 63737,
				728, 166, 63210, 8226, 99, 263, 711, 8629, 269, 231,
				265, 267, 184, 162, 63199, 63394, 63200, 967, 9675, 8855,
				8853, 710, 9827, 58, 8353, 44, 63171, 63201, 63202, 8773,
				169, 63721, 63193, 164, 63185, 63186, 63188, 63189, 100, 8224,
				8225, 63187, 63190, 271, 273, 176, 948, 9830, 168, 63191,
				63192, 901, 247, 9619, 9604, 36, 63203, 63268, 63204, 8363,
				729, 803, 305, 63166, 8901, 63211, 101, 233, 277, 283,
				234, 235, 279, 232, 56, 8328, 63288, 8312, 8712, 8230,
				275, 8212, 8709, 8211, 331, 281, 949, 941, 61, 8801,
				8494, 63212, 951, 942, 240, 33, 8252, 161, 63393, 63265,
				8707, 102, 9792, 64256, 64259, 64260, 64257, 8210, 9632, 9644,
				53, 8541, 8325, 63285, 8309, 64258, 402, 52, 8324, 63284,
				8308, 8260, 8725, 8355, 103, 947, 287, 487, 285, 291,
				289, 223, 8711, 96, 768, 62, 8805, 171, 187, 8249,
				8250, 104, 295, 293, 9829, 777, 8962, 733, 45, 173,
				63205, 63206, 105, 237, 301, 238, 239, 236, 307, 299,
				8734, 8747, 8993, 63733, 8992, 8745, 9688, 9689, 9787, 303,
				953, 970, 912, 943, 63213, 297, 106, 309, 107, 954,
				311, 312, 108, 314, 955, 318, 316, 320, 60, 8804,
				9612, 8356, 63168, 8743, 172, 8744, 383, 9674, 322, 63214,
				9617, 109, 175, 713, 9794, 8722, 8242, 63215, 181, 956,
				215, 9834, 9835, 110, 324, 329, 328, 326, 57, 8329,
				63289, 8313, 8713, 8800, 8836, 8319, 241, 957, 35, 111,
				243, 335, 244, 246, 339, 731, 242, 417, 337, 333,
				969, 982, 974, 959, 972, 49, 8228, 8539, 63196, 189,
				8321, 63281, 188, 185, 8531, 9702, 170, 186, 8735, 248,
				511, 63216, 245, 112, 182, 40, 63725, 63724, 8333, 8317,
				63723, 41, 63736, 63735, 8334, 8318, 63734, 8706, 37, 46,
				183, 8729, 63207, 63208, 8869, 8240, 8359, 966, 981, 960,
				43, 177, 8478, 8719, 8834, 8835, 8733, 968, 113, 63,
				191, 63423, 63295, 34, 8222, 8220, 8221, 8216, 8219, 8217,
				8218, 39, 114, 341, 8730, 63717, 345, 343, 8838, 8839,
				174, 63720, 63194, 8976, 961, 730, 63217, 9616, 63197, 115,
				347, 353, 351, 63170, 349, 537, 8243, 167, 59, 55,
				8542, 8327, 63287, 8311, 9618, 963, 962, 8764, 54, 8326,
				63286, 8310, 47, 9786, 32, 160, 9824, 63218, 163, 8715,
				8721, 9788, 116, 964, 359, 357, 355, 539, 8756, 952,
				977, 254, 51, 8540, 8323, 63283, 190, 63198, 179, 732,
				771, 900, 8482, 63722, 63195, 9660, 9668, 9658, 9650, 63219,
				50, 8229, 8322, 63282, 178, 8532, 117, 250, 365, 251,
				252, 249, 432, 369, 363, 95, 8215, 8746, 8704, 371,
				9600, 965, 971, 944, 973, 367, 361, 118, 119, 7811,
				373, 7813, 8472, 7809, 120, 958, 121, 253, 375, 255,
				165, 7923, 122, 378, 382, 380, 48, 8320, 63280, 8304,
				950
			};
			_glyph = new string[1051]
			{
				"A", "AE", "AEacute", "AEsmall", "Aacute", "Aacutesmall", "Abreve", "Acircumflex", "Acircumflexsmall", "Acute",
				"Acutesmall", "Adieresis", "Adieresissmall", "Agrave", "Agravesmall", "Alpha", "Alphatonos", "Amacron", "Aogonek", "Aring",
				"Aringacute", "Aringsmall", "Asmall", "Atilde", "Atildesmall", "B", "Beta", "Brevesmall", "Bsmall", "C",
				"Cacute", "Caron", "Caronsmall", "Ccaron", "Ccedilla", "Ccedillasmall", "Ccircumflex", "Cdotaccent", "Cedillasmall", "Chi",
				"Circumflexsmall", "Csmall", "D", "Dcaron", "Dcroat", "Delta", "Delta", "Dieresis", "DieresisAcute", "DieresisGrave",
				"Dieresissmall", "Dotaccentsmall", "Dsmall", "E", "Eacute", "Eacutesmall", "Ebreve", "Ecaron", "Ecircumflex", "Ecircumflexsmall",
				"Edieresis", "Edieresissmall", "Edotaccent", "Egrave", "Egravesmall", "Emacron", "Eng", "Eogonek", "Epsilon", "Epsilontonos",
				"Esmall", "Eta", "Etatonos", "Eth", "Ethsmall", "Euro", "F", "Fsmall", "G", "Gamma",
				"Gbreve", "Gcaron", "Gcircumflex", "Gcommaaccent", "Gdotaccent", "Grave", "Gravesmall", "Gsmall", "H", "H18533",
				"H18543", "H18551", "H22073", "Hbar", "Hcircumflex", "Hsmall", "Hungarumlaut", "Hungarumlautsmall", "I", "IJ",
				"Iacute", "Iacutesmall", "Ibreve", "Icircumflex", "Icircumflexsmall", "Idieresis", "Idieresissmall", "Idotaccent", "Ifraktur", "Igrave",
				"Igravesmall", "Imacron", "Iogonek", "Iota", "Iotadieresis", "Iotatonos", "Ismall", "Itilde", "J", "Jcircumflex",
				"Jsmall", "K", "Kappa", "Kcommaaccent", "Ksmall", "L", "LL", "Lacute", "Lambda", "Lcaron",
				"Lcommaaccent", "Ldot", "Lslash", "Lslashsmall", "Lsmall", "M", "Macron", "Macronsmall", "Msmall", "Mu",
				"N", "Nacute", "Ncaron", "Ncommaaccent", "Nsmall", "Ntilde", "Ntildesmall", "Nu", "O", "OE",
				"OEsmall", "Oacute", "Oacutesmall", "Obreve", "Ocircumflex", "Ocircumflexsmall", "Odieresis", "Odieresissmall", "Ogoneksmall", "Ograve",
				"Ogravesmall", "Ohorn", "Ohungarumlaut", "Omacron", "Omega", "Omega", "Omegatonos", "Omicron", "Omicrontonos", "Oslash",
				"Oslashacute", "Oslashsmall", "Osmall", "Otilde", "Otildesmall", "P", "Phi", "Pi", "Psi", "Psmall",
				"Q", "Qsmall", "R", "Racute", "Rcaron", "Rcommaaccent", "Rfraktur", "Rho", "Ringsmall", "Rsmall",
				"S", "SF010000", "SF020000", "SF030000", "SF040000", "SF050000", "SF060000", "SF070000", "SF080000", "SF090000",
				"SF100000", "SF110000", "SF190000", "SF200000", "SF210000", "SF220000", "SF230000", "SF240000", "SF250000", "SF260000",
				"SF270000", "SF280000", "SF360000", "SF370000", "SF380000", "SF390000", "SF400000", "SF410000", "SF420000", "SF430000",
				"SF440000", "SF450000", "SF460000", "SF470000", "SF480000", "SF490000", "SF500000", "SF510000", "SF520000", "SF530000",
				"SF540000", "Sacute", "Scaron", "Scaronsmall", "Scedilla", "Scedilla", "Scircumflex", "Scommaaccent", "Sigma", "Ssmall",
				"T", "Tau", "Tbar", "Tcaron", "Tcommaaccent", "Tcommaaccent", "Theta", "Thorn", "Thornsmall", "Tildesmall",
				"Tsmall", "U", "Uacute", "Uacutesmall", "Ubreve", "Ucircumflex", "Ucircumflexsmall", "Udieresis", "Udieresissmall", "Ugrave",
				"Ugravesmall", "Uhorn", "Uhungarumlaut", "Umacron", "Uogonek", "Upsilon", "Upsilon1", "Upsilondieresis", "Upsilontonos", "Uring",
				"Usmall", "Utilde", "V", "Vsmall", "W", "Wacute", "Wcircumflex", "Wdieresis", "Wgrave", "Wsmall",
				"X", "Xi", "Xsmall", "Y", "Yacute", "Yacutesmall", "Ycircumflex", "Ydieresis", "Ydieresissmall", "Ygrave",
				"Ysmall", "Z", "Zacute", "Zcaron", "Zcaronsmall", "Zdotaccent", "Zeta", "Zsmall", "a", "aacute",
				"abreve", "acircumflex", "acute", "acutecomb", "adieresis", "ae", "aeacute", "afii00208", "afii10017", "afii10018",
				"afii10019", "afii10020", "afii10021", "afii10022", "afii10023", "afii10024", "afii10025", "afii10026", "afii10027", "afii10028",
				"afii10029", "afii10030", "afii10031", "afii10032", "afii10033", "afii10034", "afii10035", "afii10036", "afii10037", "afii10038",
				"afii10039", "afii10040", "afii10041", "afii10042", "afii10043", "afii10044", "afii10045", "afii10046", "afii10047", "afii10048",
				"afii10049", "afii10050", "afii10051", "afii10052", "afii10053", "afii10054", "afii10055", "afii10056", "afii10057", "afii10058",
				"afii10059", "afii10060", "afii10061", "afii10062", "afii10063", "afii10064", "afii10065", "afii10066", "afii10067", "afii10068",
				"afii10069", "afii10070", "afii10071", "afii10072", "afii10073", "afii10074", "afii10075", "afii10076", "afii10077", "afii10078",
				"afii10079", "afii10080", "afii10081", "afii10082", "afii10083", "afii10084", "afii10085", "afii10086", "afii10087", "afii10088",
				"afii10089", "afii10090", "afii10091", "afii10092", "afii10093", "afii10094", "afii10095", "afii10096", "afii10097", "afii10098",
				"afii10099", "afii10100", "afii10101", "afii10102", "afii10103", "afii10104", "afii10105", "afii10106", "afii10107", "afii10108",
				"afii10109", "afii10110", "afii10145", "afii10146", "afii10147", "afii10148", "afii10192", "afii10193", "afii10194", "afii10195",
				"afii10196", "afii10831", "afii10832", "afii10846", "afii299", "afii300", "afii301", "afii57381", "afii57388", "afii57392",
				"afii57393", "afii57394", "afii57395", "afii57396", "afii57397", "afii57398", "afii57399", "afii57400", "afii57401", "afii57403",
				"afii57407", "afii57409", "afii57410", "afii57411", "afii57412", "afii57413", "afii57414", "afii57415", "afii57416", "afii57417",
				"afii57418", "afii57419", "afii57420", "afii57421", "afii57422", "afii57423", "afii57424", "afii57425", "afii57426", "afii57427",
				"afii57428", "afii57429", "afii57430", "afii57431", "afii57432", "afii57433", "afii57434", "afii57440", "afii57441", "afii57442",
				"afii57443", "afii57444", "afii57445", "afii57446", "afii57448", "afii57449", "afii57450", "afii57451", "afii57452", "afii57453",
				"afii57454", "afii57455", "afii57456", "afii57457", "afii57458", "afii57470", "afii57505", "afii57506", "afii57507", "afii57508",
				"afii57509", "afii57511", "afii57512", "afii57513", "afii57514", "afii57519", "afii57534", "afii57636", "afii57645", "afii57658",
				"afii57664", "afii57665", "afii57666", "afii57667", "afii57668", "afii57669", "afii57670", "afii57671", "afii57672", "afii57673",
				"afii57674", "afii57675", "afii57676", "afii57677", "afii57678", "afii57679", "afii57680", "afii57681", "afii57682", "afii57683",
				"afii57684", "afii57685", "afii57686", "afii57687", "afii57688", "afii57689", "afii57690", "afii57694", "afii57695", "afii57700",
				"afii57705", "afii57716", "afii57717", "afii57718", "afii57723", "afii57793", "afii57794", "afii57795", "afii57796", "afii57797",
				"afii57798", "afii57799", "afii57800", "afii57801", "afii57802", "afii57803", "afii57804", "afii57806", "afii57807", "afii57839",
				"afii57841", "afii57842", "afii57929", "afii61248", "afii61289", "afii61352", "afii61573", "afii61574", "afii61575", "afii61664",
				"afii63167", "afii64937", "agrave", "aleph", "alpha", "alphatonos", "amacron", "ampersand", "ampersandsmall", "angle",
				"angleleft", "angleright", "anoteleia", "aogonek", "approxequal", "aring", "aringacute", "arrowboth", "arrowdblboth", "arrowdbldown",
				"arrowdblleft", "arrowdblright", "arrowdblup", "arrowdown", "arrowhorizex", "arrowleft", "arrowright", "arrowup", "arrowupdn", "arrowupdnbse",
				"arrowvertex", "asciicircum", "asciitilde", "asterisk", "asteriskmath", "abaseior", "at", "atilde", "b", "backslash",
				"bar", "beta", "block", "braceex", "braceleft", "braceleftbt", "braceleftmid", "bracelefttp", "braceright", "bracerightbt",
				"bracerightmid", "bracerighttp", "bracketleft", "bracketleftbt", "bracketleftex", "bracketlefttp", "bracketright", "bracketrightbt", "bracketrightex", "bracketrighttp",
				"breve", "brokenbar", "bbaseior", "bullet", "c", "cacute", "caron", "carriagereturn", "ccaron", "ccedilla",
				"ccircumflex", "cdotaccent", "cedilla", "cent", "centinferior", "centoldstyle", "centbaseior", "chi", "circle", "circlemultiply",
				"circleplus", "circumflex", "club", "colon", "colonmonetary", "comma", "commaaccent", "commainferior", "commabaseior", "congruent",
				"copyright", "copyrightsans", "copyrightserif", "currency", "cyrBreve", "cyrFlex", "cyrbreve", "cyrflex", "d", "dagger",
				"daggerdbl", "dblGrave", "dblgrave", "dcaron", "dcroat", "degree", "delta", "diamond", "dieresis", "dieresisacute",
				"dieresisgrave", "dieresistonos", "divide", "dkshade", "dnblock", "dollar", "dollarinferior", "dollaroldstyle", "dollarbaseior", "dong",
				"dotaccent", "dotbelowcomb", "dotlessi", "dotlessj", "dotmath", "dbaseior", "e", "eacute", "ebreve", "ecaron",
				"ecircumflex", "edieresis", "edotaccent", "egrave", "eight", "eightinferior", "eightoldstyle", "eightbaseior", "element", "ellipsis",
				"emacron", "emdash", "emptyset", "endash", "eng", "eogonek", "epsilon", "epsilontonos", "equal", "equivalence",
				"estimated", "ebaseior", "eta", "etatonos", "eth", "exclam", "exclamdbl", "exclamdown", "exclamdownsmall", "exclamsmall",
				"existential", "f", "female", "ff", "ffi", "ffl", "fi", "figuredash", "filledbox", "filledrect",
				"five", "fiveeighths", "fiveinferior", "fiveoldstyle", "fivebaseior", "fl", "florin", "four", "fourinferior", "fouroldstyle",
				"fourbaseior", "fraction", "fraction", "franc", "g", "gamma", "gbreve", "gcaron", "gcircumflex", "gcommaaccent",
				"gdotaccent", "germandbls", "gradient", "grave", "gravecomb", "greater", "greaterequal", "guillemotleft", "guillemotright", "guilsinglleft",
				"guilsinglright", "h", "hbar", "hcircumflex", "heart", "hookabovecomb", "house", "hungarumlaut", "hyphen", "hyphen",
				"hypheninferior", "hyphenbaseior", "i", "iacute", "ibreve", "icircumflex", "idieresis", "igrave", "ij", "imacron",
				"infinity", "integral", "integralbt", "integralex", "integraltp", "intersection", "invbullet", "invcircle", "invsmileface", "iogonek",
				"iota", "iotadieresis", "iotadieresistonos", "iotatonos", "ibaseior", "itilde", "j", "jcircumflex", "k", "kappa",
				"kcommaaccent", "kgreenlandic", "l", "lacute", "lambda", "lcaron", "lcommaaccent", "ldot", "less", "lessequal",
				"lfblock", "lira", "ll", "logicaland", "logicalnot", "logicalor", "longs", "lozenge", "lslash", "lbaseior",
				"ltshade", "m", "macron", "macron", "male", "minus", "minute", "mbaseior", "mu", "mu",
				"multiply", "musicalnote", "musicalnotedbl", "n", "nacute", "napostrophe", "ncaron", "ncommaaccent", "nine", "nineinferior",
				"nineoldstyle", "ninebaseior", "notelement", "notequal", "notsubset", "nbaseior", "ntilde", "nu", "numbersign", "o",
				"oacute", "obreve", "ocircumflex", "odieresis", "oe", "ogonek", "ograve", "ohorn", "ohungarumlaut", "omacron",
				"omega", "omega1", "omegatonos", "omicron", "omicrontonos", "one", "onedotenleader", "oneeighth", "onefitted", "onehalf",
				"oneinferior", "oneoldstyle", "onequarter", "onebaseior", "onethird", "openbullet", "ordfeminine", "ordmasculine", "orthogonal", "oslash",
				"oslashacute", "obaseior", "otilde", "p", "paragraph", "parenleft", "parenleftbt", "parenleftex", "parenleftinferior", "parenleftbaseior",
				"parenlefttp", "parenright", "parenrightbt", "parenrightex", "parenrightinferior", "parenrightbaseior", "parenrighttp", "partialdiff", "percent", "period",
				"periodcentered", "periodcentered", "periodinferior", "periodbaseior", "perpendicular", "perthousand", "peseta", "phi", "phi1", "pi",
				"plus", "plusminus", "prescription", "product", "propersubset", "properbaseset", "proportional", "psi", "q", "question",
				"questiondown", "questiondownsmall", "questionsmall", "quotedbl", "quotedblbase", "quotedblleft", "quotedblright", "quoteleft", "quotereversed", "quoteright",
				"quotesinglbase", "quotesingle", "r", "racute", "radical", "radicalex", "rcaron", "rcommaaccent", "reflexsubset", "reflexbaseset",
				"registered", "registersans", "registerserif", "revlogicalnot", "rho", "ring", "rbaseior", "rtblock", "rupiah", "s",
				"sacute", "scaron", "scedilla", "scedilla", "scircumflex", "scommaaccent", "second", "section", "semicolon", "seven",
				"seveneighths", "seveninferior", "sevenoldstyle", "sevenbaseior", "shade", "sigma", "sigma1", "similar", "six", "sixinferior",
				"sixoldstyle", "sixbaseior", "slash", "smileface", "space", "space", "spade", "sbaseior", "sterling", "suchthat",
				"summation", "sun", "t", "tau", "tbar", "tcaron", "tcommaaccent", "tcommaaccent", "therefore", "theta",
				"theta1", "thorn", "three", "threeeighths", "threeinferior", "threeoldstyle", "threequarters", "threequartersemdash", "threebaseior", "tilde",
				"tildecomb", "tonos", "trademark", "trademarksans", "trademarkserif", "triagdn", "triaglf", "triagrt", "triagup", "tbaseior",
				"two", "twodotenleader", "twoinferior", "twooldstyle", "twobaseior", "twothirds", "u", "uacute", "ubreve", "ucircumflex",
				"udieresis", "ugrave", "uhorn", "uhungarumlaut", "umacron", "underscore", "underscoredbl", "union", "universal", "uogonek",
				"upblock", "upsilon", "upsilondieresis", "upsilondieresistonos", "upsilontonos", "uring", "utilde", "v", "w", "wacute",
				"wcircumflex", "wdieresis", "weierstrass", "wgrave", "x", "xi", "y", "yacute", "ycircumflex", "ydieresis",
				"yen", "ygrave", "z", "zacute", "zcaron", "zdotaccent", "zero", "zeroinferior", "zerooldstyle", "zerobaseior",
				"zeta"
			};
			_pdfcode = new int[1051]
			{
				101, 306, 0, 346, 301, 341, 0, 302, 342, 264,
				0, 304, 344, 300, 340, 0, 0, 0, 0, 305,
				0, 345, 141, 303, 343, 102, 0, 0, 142, 103,
				0, 0, 0, 0, 307, 347, 0, 0, 0, 0,
				0, 143, 104, 0, 0, 0, 0, 250, 0, 0,
				0, 0, 144, 105, 311, 351, 0, 0, 312, 352,
				313, 353, 0, 310, 350, 0, 0, 0, 0, 0,
				145, 0, 0, 320, 360, 200, 106, 146, 107, 0,
				0, 0, 0, 0, 0, 140, 0, 147, 110, 0,
				0, 0, 0, 0, 0, 150, 0, 0, 111, 0,
				315, 355, 0, 316, 356, 317, 357, 0, 0, 314,
				354, 0, 0, 0, 0, 0, 151, 0, 112, 0,
				152, 113, 0, 0, 153, 114, 0, 0, 0, 0,
				0, 0, 0, 0, 154, 115, 257, 0, 155, 265,
				116, 0, 0, 0, 156, 321, 361, 0, 117, 214,
				234, 323, 363, 0, 324, 364, 326, 366, 0, 322,
				362, 0, 0, 0, 0, 0, 0, 0, 0, 330,
				0, 370, 157, 325, 365, 120, 0, 0, 0, 160,
				121, 161, 122, 0, 0, 0, 0, 0, 0, 162,
				123, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 212, 232, 0, 0, 0, 0, 0, 163,
				124, 0, 0, 0, 0, 0, 0, 336, 376, 0,
				164, 125, 332, 372, 0, 333, 373, 334, 374, 331,
				371, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				165, 0, 126, 166, 127, 0, 0, 0, 0, 167,
				130, 0, 170, 131, 335, 375, 0, 237, 377, 0,
				171, 132, 0, 216, 236, 0, 0, 172, 101, 301,
				0, 302, 264, 0, 304, 306, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 300, 0, 0, 0, 0, 46, 0, 0,
				0, 0, 0, 0, 0, 305, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 136, 176, 52, 0, 0, 100, 303, 102, 134,
				174, 0, 0, 0, 173, 0, 0, 0, 175, 0,
				0, 0, 133, 0, 0, 0, 135, 0, 0, 0,
				0, 246, 0, 225, 103, 0, 0, 0, 0, 307,
				0, 0, 270, 242, 0, 0, 0, 0, 0, 0,
				0, 210, 0, 72, 0, 54, 0, 0, 0, 0,
				251, 0, 0, 244, 0, 0, 0, 0, 104, 206,
				207, 0, 0, 0, 0, 260, 0, 0, 250, 0,
				0, 0, 367, 0, 0, 44, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 105, 311, 0, 0,
				312, 313, 0, 310, 70, 0, 0, 0, 0, 205,
				0, 227, 0, 226, 0, 0, 0, 0, 75, 0,
				0, 0, 0, 0, 320, 41, 0, 241, 0, 0,
				0, 106, 0, 0, 0, 0, 0, 0, 0, 0,
				65, 0, 0, 0, 0, 0, 203, 64, 0, 0,
				0, 0, 0, 0, 107, 0, 0, 0, 0, 0,
				0, 337, 0, 140, 0, 76, 0, 253, 273, 213,
				233, 110, 0, 0, 0, 0, 0, 0, 55, 55,
				0, 0, 111, 315, 0, 316, 317, 314, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 112, 0, 113, 0,
				0, 0, 114, 0, 0, 0, 0, 0, 74, 0,
				0, 0, 0, 0, 254, 0, 0, 0, 0, 0,
				0, 115, 257, 257, 0, 0, 0, 0, 265, 265,
				327, 0, 0, 116, 0, 0, 0, 0, 71, 0,
				0, 0, 0, 0, 0, 0, 321, 0, 43, 117,
				323, 0, 324, 326, 214, 0, 322, 0, 0, 0,
				0, 0, 0, 0, 0, 61, 0, 0, 0, 275,
				0, 0, 274, 0, 0, 0, 252, 272, 0, 330,
				0, 0, 325, 120, 266, 50, 0, 0, 0, 0,
				0, 51, 0, 0, 0, 0, 0, 0, 45, 56,
				267, 267, 0, 0, 0, 211, 0, 0, 0, 0,
				53, 261, 0, 0, 0, 0, 0, 0, 121, 77,
				277, 0, 0, 42, 204, 223, 224, 221, 0, 222,
				202, 47, 122, 0, 0, 0, 0, 0, 0, 0,
				256, 0, 0, 0, 0, 0, 0, 0, 0, 123,
				0, 212, 0, 0, 0, 0, 0, 247, 73, 67,
				0, 0, 0, 0, 0, 0, 0, 0, 66, 0,
				0, 0, 57, 0, 40, 40, 0, 0, 243, 0,
				0, 0, 124, 0, 0, 0, 0, 0, 0, 0,
				0, 336, 63, 0, 0, 0, 276, 0, 0, 230,
				0, 0, 231, 0, 0, 0, 0, 0, 0, 0,
				62, 0, 0, 0, 0, 0, 125, 332, 0, 333,
				334, 331, 0, 0, 0, 137, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 126, 127, 0,
				0, 0, 0, 0, 130, 0, 131, 335, 0, 237,
				245, 0, 132, 0, 216, 0, 60, 0, 0, 0,
				0
			};
			_UniToGlyph = new Hashtable();
			_UniToPDFCode = new Hashtable();
			_GlyphToUni = new Hashtable();
			_GlyphToPDFCode = new Hashtable();
			for (int i = 0; i < _unicode.Length; i++)
			{
				if (!_UniToGlyph.ContainsKey(_unicode[i]))
				{
					_UniToGlyph.Add(_unicode[i], _glyph[i]);
				}
				if (!_UniToPDFCode.ContainsKey(_unicode[i]))
				{
					_UniToPDFCode.Add(_unicode[i], _pdfcode[i]);
				}
				if (!_GlyphToUni.ContainsKey(_glyph[i]))
				{
					_GlyphToUni.Add(_glyph[i], _unicode[i]);
				}
				if (!_GlyphToPDFCode.ContainsKey(_glyph[i]))
				{
					_GlyphToPDFCode.Add(_glyph[i], _pdfcode[i]);
				}
			}
		}

		public static int UnicodeFromGlyph(string glyphName)
		{
			if (_GlyphToUni.ContainsKey(glyphName))
			{
				return Convert.ToInt32(_GlyphToUni[glyphName]);
			}
			return 0;
		}

		public static string GlyphFromUnicode(int unicodeIndex)
		{
			if (_UniToGlyph.ContainsKey(unicodeIndex))
			{
				return Convert.ToString(_UniToGlyph[unicodeIndex]);
			}
			return "";
		}

		public static string pdfCodeFromGlyph(string glyphName)
		{
			int num = 0;
			if (_GlyphToPDFCode.ContainsKey(glyphName))
			{
				num = Convert.ToInt32(_GlyphToPDFCode[glyphName]);
			}
			if (num != 0)
			{
				return "\\" + num;
			}
			return "";
		}

		public static string pdfCodeFromUnicode(int unicodeIndex)
		{
			int num = 0;
			if (_UniToPDFCode.ContainsKey(unicodeIndex))
			{
				num = Convert.ToInt32(_UniToPDFCode[unicodeIndex]);
			}
			if (num != 0)
			{
				return "\\" + num;
			}
			return "";
		}
	}
}
