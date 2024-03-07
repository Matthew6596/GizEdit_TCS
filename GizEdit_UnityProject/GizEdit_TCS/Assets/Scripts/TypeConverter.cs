using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEditor.PackageManager;
using System.Web;
using UnityEngine.UI;

public class TypeConverter : MonoBehaviour
{
    static public string Int8ToHex(uint n)
    {
        string r="";

        //I do not care how dumb this is
        switch (n) {
            case(0):r = "00"; break; case(64):r = "40"; break; case(128):r = "80"; break; case(192):r = "C0"; break;
            case (1):r = "01"; break; case(65):r = "41"; break; case(129):r = "81"; break; case(193):r = "C1"; break;
            case(2):r = "02"; break; case(66):r = "42"; break; case(130):r = "82"; break; case(194):r = "C2"; break;
            case(3):r = "03"; break; case(67):r = "43"; break; case(131):r = "83"; break; case(195):r = "C3"; break;
            case(4):r = "04"; break; case(68):r = "44"; break; case(132):r = "84"; break; case(196):r = "C4"; break;
            case(5):r = "05"; break; case(69):r = "45"; break; case(133):r = "85"; break; case(197):r = "C5"; break;
            case(6):r = "06"; break; case(70):r = "46"; break; case(134):r = "86"; break; case(198):r = "C6"; break;
            case(7):r = "07"; break; case(71):r = "47"; break; case(135):r = "87"; break; case(199):r = "C7"; break;
            case(8):r = "08"; break; case(72):r = "48"; break; case(136):r = "88"; break; case(200):r = "C8"; break;
            case(9):r = "09"; break; case(73):r = "49"; break; case(137):r = "89"; break; case(201):r = "C9"; break;
            case(10):r = "0A"; break; case(74):r = "4A"; break; case(138):r = "8A"; break; case(202):r = "CA"; break;
            case(11):r = "0B"; break; case(75):r = "4B"; break; case(139):r = "8B"; break; case(203):r = "CB"; break;
            case(12):r = "0C"; break; case(76):r = "4C"; break; case(140):r = "8C"; break; case(204):r = "CC"; break;
            case(13):r = "0D"; break; case(77):r = "4D"; break; case(141):r = "8D"; break; case(205):r = "CD"; break;
            case(14):r = "0E"; break; case(78):r = "4E"; break; case(142):r = "8E"; break; case(206):r = "CE"; break;
            case(15):r = "0F"; break; case(79):r = "4F"; break; case(143):r = "8F"; break; case(207):r = "CF"; break;
            case(16):r = "10"; break; case(80):r = "50"; break; case(144):r = "90"; break; case(208):r = "D0"; break;
            case(17):r = "11"; break; case(81):r = "51"; break; case(145):r = "91"; break; case(209):r = "D1"; break;
            case(18):r = "12"; break; case(82):r = "52"; break; case(146):r = "92"; break; case(210):r = "D2"; break;
            case(19):r = "13"; break; case(83):r = "53"; break; case(147):r = "93"; break; case(211):r = "D3"; break;
            case(20):r = "14"; break; case(84):r = "54"; break; case(148):r = "94"; break; case(212):r = "D4"; break;
            case(21):r = "15"; break; case(85):r = "55"; break; case(149):r = "95"; break; case(213):r = "D5"; break;
            case(22):r = "16"; break; case(86):r = "56"; break; case(150):r = "96"; break; case(214):r = "D6"; break;
            case(23):r = "17"; break; case(87):r = "57"; break; case(151):r = "97"; break; case(215):r = "D7"; break;
            case(24):r = "18"; break; case(88):r = "58"; break; case(152):r = "98"; break; case(216):r = "D8"; break;
            case(25):r = "19"; break; case(89):r = "59"; break; case(153):r = "99"; break; case(217):r = "D9"; break;
            case(26):r = "1A"; break; case(90):r = "5A"; break; case(154):r = "9A"; break; case(218):r = "DA"; break;
            case(27):r = "1B"; break; case(91):r = "5B"; break; case(155):r = "9B"; break; case(219):r = "DB"; break;
            case(28):r = "1C"; break; case(92):r = "5C"; break; case(156):r = "9C"; break; case(220):r = "DC"; break;
            case(29):r = "1D"; break; case(93):r = "5D"; break; case(157):r = "9D"; break; case(221):r = "DD"; break;
            case(30):r = "1E"; break; case(94):r = "5E"; break; case(158):r = "9E"; break; case(222):r = "DE"; break;
            case(31):r = "1F"; break; case(95):r = "5F"; break; case(159):r = "9F"; break; case(223):r = "DF"; break;
            case(32):r = "20"; break; case(96):r = "60"; break; case(160):r = "A0"; break; case(224):r = "E0"; break;
            case(33):r = "21"; break; case(97):r = "61"; break; case(161):r = "A1"; break; case(225):r = "E1"; break;
            case(34):r = "22"; break; case(98):r = "62"; break; case(162):r = "A2"; break; case(226):r = "E2"; break;
            case(35):r = "23"; break; case(99):r = "63"; break; case(163):r = "A3"; break; case(227):r = "E3"; break;
            case (36):r = "24"; break; case(100):r = "64"; break; case(164):r = "A4"; break; case(228):r = "E4"; break;
            case (37):r = "25"; break; case(101):r = "65"; break; case(165):r = "A5"; break; case(229):r = "E5"; break;
            case (38):r = "26"; break; case(102):r = "66"; break; case(166):r = "A6"; break; case(230):r = "E6"; break;
            case (39):r = "27"; break; case(103):r = "67"; break; case(167):r = "A7"; break; case(231):r = "E7";  break;
            case(40):r = "28"; break; case(104):r = "68"; break; case(168):r = "A8"; break; case(232):r = "E8";  break;
            case(41):r = "29"; break; case(105):r = "69"; break; case(169):r = "A9"; break; case(233):r = "E9";  break;
            case(42):r = "2A"; break; case(106):r = "6A"; break; case(170):r = "AA"; break; case(234):r = "EA";  break;
            case(43):r = "2B"; break; case(107):r = "6B"; break; case(171):r = "AB"; break; case(235):r = "EB";  break;
            case(44):r = "2C"; break; case(108):r = "6C"; break; case(172):r = "AC"; break; case(236):r = "EC";  break;
            case(45):r = "2D"; break; case(109):r = "6D"; break; case(173):r = "AD"; break; case(237):r = "ED";  break;
            case(46):r = "2E"; break; case(110):r = "6E"; break; case(174):r = "AE"; break; case(238):r = "EE";  break;
            case(47):r = "2F"; break; case(111):r = "6F"; break; case(175):r = "AF"; break; case(239):r = "EF";  break;
            case(48):r = "30"; break; case(112):r = "70"; break; case(176):r = "B0"; break; case(240):r = "F0";  break;
            case(49):r = "31"; break; case(113):r = "71"; break; case(177):r = "B1"; break; case(241):r = "F1";  break;
            case(50):r = "32"; break; case(114):r = "72"; break; case(178):r = "B2"; break; case(242):r = "F2";  break;
            case(51):r = "33"; break; case(115):r = "73"; break; case(179):r = "B3"; break; case(243):r = "F3";  break;
            case(52):r = "34"; break; case(116):r = "74"; break; case(180):r = "B4"; break; case(244):r = "F4";  break;
            case(53):r = "35"; break; case(117):r = "75"; break; case(181):r = "B5"; break; case(245):r = "F5";  break;
            case(54):r = "36"; break; case(118):r = "76"; break; case(182):r = "B6"; break; case(246):r = "F6";  break;
            case(55):r = "37"; break; case(119):r = "77"; break; case(183):r = "B7"; break; case(247):r = "F7";  break;
            case(56):r = "38"; break; case(120):r = "78"; break; case(184):r = "B8"; break; case(248):r = "F8";  break;
            case(57):r = "39"; break; case(121):r = "79"; break; case(185):r = "B9"; break; case(249):r = "F9";  break;
            case(58):r = "3A"; break; case(122):r = "7A"; break; case(186):r = "BA"; break; case(250):r = "FA";  break;
            case(59):r = "3B"; break; case(123):r = "7B"; break; case(187):r = "BB"; break; case(251):r = "FB";  break;
            case(60):r = "3C"; break; case(124):r = "7C"; break; case(188):r = "BC"; break; case(252):r = "FC";  break;
            case(61):r = "3D"; break; case(125):r = "7D"; break; case(189):r = "BD"; break; case(253):r = "FD";  break;
            case(62):r = "3E"; break; case(126):r = "7E"; break; case(190):r = "BE"; break; case(254):r = "FE";  break;
            case(63):r = "3F"; break; case(127):r = "7F"; break; case(191):r = "BF"; break; case(255):r = "FF";  break;
        }
        return r;
    }

    static public string getHeader(string titleName)
    {
        if (titleName == "GizObstacle") { return "0B 00 00 00 47 69 7A 4F 62 73 74 61 63 6C 65 "; }
        else if (titleName == "GizBuildit") { return "0A 00 00 00 47 69 7A 42 75 69 6C 64 69 74 "; }
        else if (titleName == "GizForce") { return "08 00 00 00 47 69 7A 46 6F 72 63 65 "; }
        else if (titleName == "blowup") { return "06 00 00 00 62 6C 6F 77 75 70 "; }
        else if (titleName == "GizmoPickup") { return "0B 00 00 00 47 69 7A 6D 6F 50 69 63 6B 75 70 "; }
        else if (titleName == "Lever") { return "05 00 00 00 4C 65 76 65 72 "; }
        else if (titleName == "Spinner") { return "07 00 00 00 53 70 69 6E 6E 65 72 "; }
        else if (titleName == "MiniCut") { return "07 00 00 00 4D 69 6E 69 43 75 74 "; }
        else if (titleName == "Tube") { return "04 00 00 00 54 75 62 65 "; }
        else if (titleName == "ZipUp") { return "05 00 00 00 5A 69 70 55 70 "; }
        else if (titleName == "GizTurret") { return "09 00 00 00 47 69 7A 54 75 72 72 65 74 "; }
        else if (titleName == "BombGenerator") { return "0D 00 00 00 42 6F 6D 62 47 65 6E 65 72 61 74 6F 72 "; }
        else if (titleName == "Panel") { return "05 00 00 00 50 61 6E 65 6C "; }
        else if (titleName == "HatMachine") { return "0A 00 00 00 48 61 74 4D 61 63 68 69 6E 65 "; }
        else if (titleName == "PushBlocks") { return "0A 00 00 00 50 75 73 68 42 6C 6F 63 6B 73 "; }
        else if (titleName == "Torp Machine") { return "0C 00 00 00 54 6F 72 70 20 4D 61 63 68 69 6E 65 "; }
        else if (titleName == "ShadowEditor") { return "0C 00 00 00 53 68 61 64 6F 77 45 64 69 74 6F 72 "; }
        else if (titleName == "Grapple") { return "07 00 00 00 47 72 61 70 70 6C 65 "; }
        else if (titleName == "Plug") { return "04 00 00 00 50 6C 75 67 "; }
        else if (titleName == "Techno") { return "06 00 00 00 54 65 63 68 6E 6F "; }
        else { return ""; }
    }
    static public string[] headerHex =
    {
        "0B 00 00 00 47 69 7A 4F 62 73 74 61 63 6C 65 ",
        "0A 00 00 00 47 69 7A 42 75 69 6C 64 69 74 ",
        "08 00 00 00 47 69 7A 46 6F 72 63 65 ",
        "06 00 00 00 62 6C 6F 77 75 70 ",
        "0B 00 00 00 47 69 7A 6D 6F 50 69 63 6B 75 70 ",
        "05 00 00 00 4C 65 76 65 72 ",
        "07 00 00 00 53 70 69 6E 6E 65 72 ",
        "07 00 00 00 4D 69 6E 69 43 75 74 ",
        "04 00 00 00 54 75 62 65 ",
        "05 00 00 00 5A 69 70 55 70 ",
        "09 00 00 00 47 69 7A 54 75 72 72 65 74 ",
        "0D 00 00 00 42 6F 6D 62 47 65 6E 65 72 61 74 6F 72 ",
        "05 00 00 00 50 61 6E 65 6C ",
        "0A 00 00 00 48 61 74 4D 61 63 68 69 6E 65 ",
        "0A 00 00 00 50 75 73 68 42 6C 6F 63 6B 73 ",
        "0C 00 00 00 54 6F 72 70 20 4D 61 63 68 69 6E 65 ",
        "0C 00 00 00 53 68 61 64 6F 77 45 64 69 74 6F 72 ",
        "07 00 00 00 47 72 61 70 70 6C 65 ",
        "04 00 00 00 50 6C 75 67 ",
        "06 00 00 00 54 65 63 68 6E 6F ",
    };

    static public uint hexCharToInt(uint _char)
    {
        switch (_char)
        {
            case (48): _char = 0; break;
            case (52): _char = 4; break;
            case (56): _char = 8; break;
            case (67): _char = 12; break;
            case (49): _char = 1; break;
            case (53): _char = 5; break;
            case (57): _char = 9; break;
            case (68): _char = 13; break;
            case (50): _char = 2; break;
            case (54): _char = 6; break;
            case (65): _char = 10; break;
            case (69): _char = 14; break;
            case (51): _char = 3; break;
            case (55): _char = 7; break;
            case (66): _char = 11; break;
            case (70): _char = 15; break;
        }
        return _char;
    }

    static public uint HexToInt8(string _h)
    {
        uint _v1 = hexCharToInt(_h[0]);
        uint _v2 = hexCharToInt(_h[1]);
        return (_v1 * 16) + _v2;
    }

    static public uint HexToInt16(string _h)
    {
        uint _v1 = hexCharToInt(_h[0]);
        uint _v2 = hexCharToInt(_h[1]);
        uint _v3 = hexCharToInt(_h[3]);
        uint _v4 = hexCharToInt(_h[4]);
        _v1 *= 16;
        _v3 *= 4096;
        _v4 *= 256;
        return (_v1 + _v2 + _v3 + _v4);
    }

    static public string Int16ToHex(uint _n)
    {
        uint _v1 = _n % 256;
        uint _v2 = _n - _v1;
        string _h1 = Int8ToHex(_v1);
        string _h2 = Int8ToHex(_v2/256);
        return (_h1+" " + _h2 + " ");
    }

    static public uint HexToInt32(string _h)
    {
        uint _v1 = hexCharToInt(_h[0]);
        uint _v2 = hexCharToInt(_h[1]);
        uint _v3 = hexCharToInt(_h[3]);
        uint _v4 = hexCharToInt(_h[4]);
        uint _v5 = hexCharToInt(_h[6]);
        uint _v6 = hexCharToInt(_h[7]);
        uint _v7 = hexCharToInt(_h[9]);
        uint _v8 = hexCharToInt(_h[10]);
        _v1 *= 16;
        _v3 *= 4096;
        _v4 *= 256;
        _v5 *= 1048576;
        _v6 *= 65536;
        _v7 *= 268435456;
        _v8 *= 16777216;
        return (_v1 + _v2 + _v3 + _v4 + _v5 + _v6 + _v7 + _v8);
    }

    static public string Int32ToHex(uint _n)
    {
        uint v1 = _n % 65536;
        uint v2 = _n - v1;
        string h1 = Int16ToHex(v1);
        string h2 = Int16ToHex(v2/65536);
        return (h1+h2);
    }

    static public float HexToFloat32(string _h, bool round = false)
    {
        string str = "";
        str += _h[9]; str += _h[10]; str += _h[6]; str += _h[7]; str += _h[3]; str += _h[4]; str += _h[0]; str += _h[1];

        uint num = uint.Parse(str, System.Globalization.NumberStyles.AllowHexSpecifier);

        byte[] floatVals = BitConverter.GetBytes(num);
        float f = BitConverter.ToSingle(floatVals, 0);

        if (round){f = Mathf.Round(f*1000)/1000;}
        return f;
    }

    static public string Float32ToHex(float _n)
    {
        var bytes = BitConverter.GetBytes(_n);
        var i = BitConverter.ToInt32(bytes, 0);
        string _h = i.ToString("X8");
        string h1=_h[..2], h2=_h.Substring(2,2),h3=_h.Substring(4,2),h4=_h.Substring(6,2);
        return (h4 + " " + h3 + " " + h2 + " " + h1 + " ");
    }

    static public string HexToString(string _h)
    {
        string ret = "";
        for (int j = 0; j < _h.Length; j++)
        {
            if (_h[j] == ' ') j++;
            if (j >= _h.Length) break;
            uint _c = HexToInt8(_h.Substring(j, 2));
            char strChar = (char)_c;
            ret += strChar;
            j++;
        }
        return ret;
    }

    static public string StringToHex(string _h,int _length=0)
    {
        int _l = _length - _h.Length;
        string ret = "";
        for(int j=0; j<_h.Length; j++)
        {
            char strChar = _h[j];
            uint _c = strChar;
            ret += Int8ToHex(_c) + " ";
        }
        for (int j = 0; j < _l; j++)
        {
            ret += "00 ";
        }
        return ret;
    }

    static public float Int8AngleToFloat(uint angle)
    {
        return ((float)angle * 360f) / 256f;
    }
    static public uint FloatToInt8Angle(float angle)
    {
        return (uint)(angle * 256 / 360);
    }
    static public string SetStringSlice(string ogStr, string newStr, int start, int length)
    {
        string ret=ogStr[..start];
        for(int l=0; l<length; l++)
        {
            ret += newStr[l];
        }
        return ret+ogStr[(start + length)..];
    }

    static public void Prop_SetLabel(GameObject _prop, string _txt)
    {
        _prop.transform.GetChild(0).GetChild(0).gameObject.GetComponent<TMP_Text>().text = _txt;
    }
    static public void Prop_SetInputField<T>(GameObject _prop, T _val)
    {
        _prop.transform.GetChild(1).gameObject.GetComponent<TMP_InputField>().text = _val.ToString();
    }
    static public void Prop_SetDropdown(GameObject _prop, string[] _options, int _selectedVal = 0)
    {
        TMP_Dropdown _d = _prop.transform.GetChild(1).gameObject.GetComponent<TMP_Dropdown>();
        //_d.ClearOptions();
        List<TMP_Dropdown.OptionData> _ol = new();
        for(int _i=0; _i<_options.Length; _i++)
        {
            TMP_Dropdown.OptionData _o = new();
            _o.text = _options[_i];
            _ol.Add(_o);
        }
        _d.options = _ol;
        _d.value = _selectedVal;
    }
    static public void Prop_SetVec3(GameObject _prop, Vector3 _vec)
    {
        Transform _p = _prop.transform;
        _p.GetChild(1).gameObject.GetComponent<TMP_InputField>().text = _vec.x.ToString();
        _p.GetChild(2).gameObject.GetComponent<TMP_InputField>().text = _vec.y.ToString();
        _p.GetChild(3).gameObject.GetComponent<TMP_InputField>().text = _vec.z.ToString();
    }
    static public void Prop_SetToggle(GameObject _prop, bool _bool)
    {
        _prop.transform.GetChild(1).gameObject.GetComponent<Toggle>().isOn = _bool;
    }
    static public string Prop_GetInputField(GameObject _prop)
    {
        return _prop.transform.GetChild(1).gameObject.GetComponent<TMP_InputField>().text;
    }
    static public int Prop_GetDropdown(GameObject _prop)
    {
        return _prop.transform.GetChild(1).gameObject.GetComponent<TMP_Dropdown>().value;
    }
    static public Vector3 Prop_GetVec3(GameObject _prop)
    {
        Transform _p = _prop.transform;
        Vector3 ret = new();
        ret.x = float.Parse(_p.GetChild(1).gameObject.GetComponent<TMP_InputField>().text);
        ret.y = float.Parse(_p.GetChild(2).gameObject.GetComponent<TMP_InputField>().text);
        ret.z = float.Parse(_p.GetChild(3).gameObject.GetComponent<TMP_InputField>().text);
        return ret;
    }
    static public bool Prop_GetToggle(GameObject _prop)
    {
        return _prop.transform.GetChild(1).gameObject.GetComponent<Toggle>().isOn;
    }

    static public Transform Child_GetParent(GameObject _prop)
    {
        return (_prop.transform.GetChild(0).GetChild(0).GetChild(0));
    }
    static public GameObject Child_GetSelectDrop(GameObject _prop)
    {
        return Child_GetParent(_prop).GetChild(1).GetChild(1).gameObject;
    }
    static public Transform Child_GetBtns(GameObject _prop)
    {
        return Child_GetParent(_prop).GetChild(2);
    }
    static public int Child_GetSelected(GameObject _prop)
    {
        return Child_GetSelectDrop(_prop).GetComponent<TMP_Dropdown>().value;
    }
    static public GameObject Child_GetPropParent(GameObject _prop,int _index)
    {
        return Child_GetParent(_prop).GetChild(_index + 3).gameObject;
    }
    static public List<GameObject> Child_GetProperties(GameObject _prop)
    {
        List<GameObject> _ps = new();
        Transform _c = Child_GetParent(_prop);
        int cnt = _c.childCount;
        for(int _j = 3; _j<cnt; _j++)
        {
            _ps.Add(_c.GetChild(_j).gameObject);
        }
        return _ps;
    }
}
