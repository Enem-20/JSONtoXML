using System;
using System.Collections.Generic;
using System.Text;

namespace JSONtoXML.Parser
{
    class Validator
    {
        //bool Open_Close(string str)
        //{
        //    bool openObj = false;
        //    bool closeObj = false;

        //    bool openMas = false;
        //    bool closeMas = false;

        //    bool open = false;

        //    char prevOperator = '0';
        //    for(int i = 0; i < str.Length; ++i)
        //    {
        //        switch(str[i])
        //        {
        //            case '{':
        //                if ((!openObj && !closeObj) && (!openMas && !closeMas) && !open)
        //                    openObj = true;
        //                else
        //                    return false;

        //                break;
        //            case '}':
        //                if (openObj && (!openMas && !closeMas) && !open) { openObj = false; closeObj = false; }
        //                else
        //                    return false;

        //                break;

        //            case '[':
        //                if(prevOperator == '')
        //                if ((!openMas && !closeMas) && (!openObj && !closeObj) && !open)
        //                    openObj = true;
        //                else
        //                    return false;

        //                break;
        //            case ']':
        //                if (openMas && (!openObj && !closeObj) && !open) { openMas = false; closeMas = false; }
        //                else
        //                    return false;

        //                break;
        //            case '"':
        //                open = open ? false : true; 
        //                break;

        //            case ',':
        //                prevOperator = ',';
        //                break;
        //        }
        //    }

        //    return (!openObj && !closeObj) ? true : false;
        //}
    }
}
