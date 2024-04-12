using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HexValidator : TMP_InputValidator
{
    public override char Validate(ref string text, ref int pos, char ch)
    {
        if ((ch >= '0' && ch <= '9')||(ch>='A'&&ch<='F')||ch==' ')
        {
            text += ch;
            pos += 1;
            return ch;
        }else if (ch >= 'a' && ch <= 'f')
        {
            string str = ch.ToString();
            str.ToUpper();
            ch = str[0];
            text += ch;
            pos += 1;
            return ch;
        }
        return (char)0;
    }

}
