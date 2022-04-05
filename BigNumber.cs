using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Numerics;

// To control infinite Integer
// Only positibe number

public class BigNumber
{
    private static string[] placeValue = new string[] { "", "만","억", "조", "경", "해", "자", "양", "가", "구", "간" };
    private static int MAX = placeValue.Length;

    private int[] value = new int[MAX];
    private float fractionPart; // 소수점 아래 파트 

    public static BigNumber Zero = new BigNumber(0);
    // =================================================================================================    
    // Constructor
    public BigNumber()  {   }

    public BigNumber(BigInteger num)
    {
        setValue(num);
    }

    public BigNumber(int num)
    {
        setValue(num);
    }

    public BigNumber(float num)
    {
        setValue((int)num);
    }

    public BigNumber(string num){
        setValue(num);
    }

    public BigNumber(double num)
    {
        setValue((int)num);
    }

    // copy constructor
    public BigNumber(BigNumber num){
        for(int i=0;i<MAX;i++){
            this.value[i] = num.value[i];
        }
    }

    // =================================================================================================


    // =================================================================================================
    // Getter Setter
    public void setValue(BigInteger num)
    {
        string str = num.ToString();
        int idx = 0;

        while (!str.Equals(""))
        {
            int startIdx = str.Length - 4;
            if (startIdx < 0)
            {
                startIdx = 0;
            }

            string unit = str.Substring(startIdx);
            value[idx++] = int.Parse(unit);
            str = str.Substring(0, str.Length - unit.Length);
        }
    }

    
    public void setValue(int num)
    {
        int idx =0 ;
        while(num>0){
            value[idx++] = num % 10000;
            num /= 10000;
        }
    }

    public void setValue(string str)
    {
        int idx = 0;

        while (!str.Equals(""))
        {
            int startIdx = str.Length - 4;
            if (startIdx < 0)
            {
                startIdx = 0;
            }

            string unit = str.Substring(startIdx);
            value[idx++] = int.Parse(unit);
            str = str.Substring(0, str.Length - unit.Length);
        }
    }

    public void setValue(BigNumber num)
    {
        for(int i=0;i<MAX;i++){
            this.value[i] = num.value[i];
        }
    }

    // float 받아서 세팅
    // 소수점 아래 정확하지 않음
    // 수정 필요
    /*
    public void setValue(float num)
    {
        string strNum = num.ToString();
        int decimalPoint = strNum.IndexOf('.');

        string intPart = strNum.Substring(0,decimalPoint);
        string decimalPlacesPart ="";
        if(strNum.Contains(".")) decimalPlacesPart = strNum.Substring(decimalPoint+1);

        int idx = 0;

        while (!intPart.Equals(""))
        {
            int startIdx = intPart.Length - 4;
            if (startIdx < 0)
            {
                startIdx = 0;
            }

            string unit = intPart.Substring(startIdx);
            value[idx++] = int.Parse(unit);
            intPart = intPart.Substring(0, intPart.Length - unit.Length);
        }

        if(!decimalPlacesPart.Equals("")){
            this.fractionPart = int.Parse(decimalPlacesPart);
        }
    }
    */

    public void SetMaxValue(){
        for(int i=0;i<MAX;i++){
            value[i] = 9999;
        }
        return;
    }

    // 단위와 함께 출력
    public string getValueWithText()
    {
        if(isZero()) return "0";

        string res = "";
        int startIdx = MAX - 1;

        while (startIdx > 0 && value[startIdx] == 0)
        {
            startIdx--;
        }

        for (int i = startIdx; i >= 0 && i > startIdx - 2; i--)
        {
            if (value[i] == 0) {
                continue;
            }
     
            res += value[i] + placeValue[i];
        }

        return res;
    }

    // 정수 형태로 반환
    public BigInteger ToBigInteger()
    {
        if(isZero()) return 0;

        string res = "";
        int startIdx = MAX - 1;

        while (startIdx > 0 && value[startIdx] == 0)
        {
            startIdx--;
        }

        for (int i = startIdx; i >= 0; i--)
        {
            if (value[i] == 0) {
                res += "0000";
            }
            else{
                res += value[i];
            }
        }
        return BigInteger.Parse(res);
    }

    // 문자열 형태로 반환
    public override string ToString()
    {
        if(isZero()) return "0";

        string res = "";
        int startIdx = MAX - 1;

        while (startIdx > 0 && value[startIdx] == 0)
        {
            startIdx--;
        }

        for (int i = startIdx; i >= 0; i--)
        {
            if (value[i] == 0) {
                res += "0000";
            }
            else{
                res += value[i];
            }
        }

        if(fractionPart > 0){
            res += '.' + fractionPart.ToString();
        }

        return res;
    }
    // =================================================================================================


    // =================================================================================================
    // Status operations
    public bool isZero()
    {
        foreach(int n in value)
        {
            if (n != 0)
            {
                return false;
            }
        }

        return true;
    }

    public override bool Equals(object obj)
    {
        if(obj==null) return false;
        BigNumber other = obj as BigNumber;
        for (int i = 0; i < MAX; i++)
        {
            if (this.value[i] != other.value[i]) return false;
        }

        return true;
    }

    public override int GetHashCode()
    {
            unchecked
            {
                var hashCode = value[0].GetHashCode();
                hashCode = (hashCode * 397) ^ value[0].GetHashCode();
                hashCode = (hashCode * 397) ^ value[1].GetHashCode();
                hashCode = (hashCode * 397) ^ value[2].GetHashCode();
                return hashCode;
            }
    }

    // 두 수 비교 함수 
    private int greaterThan(BigNumber other)
    {
        // 두 수 같으면 0
        // this가 other보다 크면 1
        // this가 other보다 작으면 -1
        for(int i=0; i<MAX;i++){
            if(this.value[i] > other.value[i]) return 1;
            else if(this.value[i] < other.value[i]) return -1;
        }

        return 0;
    }

    
    // =================================================================================================


    // =================================================================================================
    // operator overloading


    
    // -----------------------------------------------------------------------------------------
    // Arithmetic Operator

    public static BigNumber operator +(BigNumber a, BigNumber b){
        int regroup = 0; // 받아올림

        BigNumber c = new BigNumber(a);

        for (int i = 0; i < MAX; i++)
        {
            if (b.value[i] == 0 && regroup == 0)
            {
                continue;
            }

            c.value[i] += b.value[i] + regroup;
            if (c.value[i] > 9999)
            {
                if(i==MAX-1){
                    c.SetMaxValue();
                    break;
                }
                regroup = 1;
                c.value[i] -= 10000;
            }
            else
            {
                regroup = 0;
            }
        }

        return c;
    }

    // bignumber 하나 더 생성 안하고 int 그대로 처리 가능하게 수정
    public static BigNumber operator +(BigNumber a, int b){
        int regroup = 0; // 받아올림

        BigNumber c = new BigNumber(a);
        BigNumber tempB = new BigNumber(b);

        for (int i = 0; i < MAX; i++)
        {
            if (tempB.value[i] == 0 && regroup == 0)
            {
                continue;
            }

            c.value[i] += tempB.value[i] + regroup;
            if (c.value[i] > 9999)
            {
                regroup = 1;
                c.value[i] -= 10000;
            }
            else
            {
                regroup = 0;
            }
        }

        return c;
    }

    public static BigNumber operator -(BigNumber a, BigNumber b){
        int regroup = 0; // 받아내림
        // if minuend is zero
        if (a==0) return a;

        BigNumber c = new BigNumber(a);

        // if minuend is less than subtrahend
        if(c<b)
        {
            for(int i = 0; i < MAX; i++)
            {
                c.value[i] = 0;
            }
            return c;
        }

        for (int i = 0; i < MAX; i++)
        {
            if (b.value[i] == 0 && regroup == 0)
            {
                continue;
            }

            if (c.value[i] < b.value[i])
            {
                c.value[i] = c.value[i] + 10000 - b.value[i] - regroup;
                regroup = 1;
            }
            else
            {
                c.value[i] = c.value[i]- b.value[i] - regroup;
                regroup = 0;
            }
        }

        return c;
    }

    public static BigNumber operator -(BigNumber a, int b){
        int regroup = 0; // 받아내림
        // if minuend is zero
        if (a==0) return a;

        BigNumber c = new BigNumber(a);
        BigNumber tempB = new BigNumber(b);
        // if minuend is less than subtrahend
        if(c<tempB)
        {
            for(int i = 0; i < MAX; i++)
            {
                c.value[i] = 0;
            }
            return c;
        }

        for (int i = 0; i < MAX; i++)
        {
            if (tempB.value[i] == 0 && regroup == 0)
            {
                continue;
            }

            if (c.value[i] < tempB.value[i])
            {
                c.value[i] = c.value[i] + 10000 - tempB.value[i] - regroup;
                regroup = 1;
            }
            else
            {
                c.value[i] = c.value[i]- tempB.value[i] - regroup;
                regroup = 0;
            }
        }

        return c;
    }

    public static BigNumber operator *(BigNumber a, int b){
        if(b < 0) return null;

        if(b==0) return new BigNumber(0);

        BigNumber c = new BigNumber(a);
        int regroup = 0;

        for (int i = 0; i < MAX; i++)
        {
            if (c.value[i] == 0 && regroup == 0)
            {
                continue;
            }

            c.value[i] = c.value[i]*b + regroup;
            if (c.value[i] > 9999)
            {
                if(i==MAX-1){
                    c.SetMaxValue();
                    break;
                }

                regroup = c.value[i] / 10000;
                c.value[i] %= 10000;
            }
            else
            {
                regroup = 0;
            }
        }

        return c;
    }

    public static BigNumber operator *(BigNumber a, float b){

        if(b < 0) return null;

        if(Mathf.Abs(b)<float.Epsilon) return new BigNumber(0);

        BigNumber c = new BigNumber(a);

        // float 유효숫자 부분이랑 BigNumber 곱해주기 
        // BigNumber에 fractionPart가 이미 있는 경우 고려해야됨
        BigInteger res = BigInteger.Multiply(a.ToBigInteger(), CustomMath.getSignificantDigits(b));
        string resToStr = res.ToString();
        c.fractionPart = float.Parse("0." + resToStr.Substring(resToStr.Length - CustomMath.getNumOfSignificantDigits(b)));
        resToStr = resToStr.Substring(0,resToStr.Length - CustomMath.getNumOfSignificantDigits(b));

        // string 네 자리씩 잘라서 넣어주기

        return c;
    }

    public static BigNumber operator *(BigNumber a, double b){
        return new BigNumber(0);
    }

    // -----------------------------------------------------------------------------------------
    // -----------------------------------------------------------------------------------------
    // Comparison Operator

    public static bool operator ==(BigNumber a, BigNumber b){
        for (int i = 0; i < MAX; i++)
        {
            if (a.value[i] != b.value[i]) return false;
        }

        return true;
    }

    public static bool operator !=(BigNumber a, BigNumber b){
        return !(a==b);
    }

    public static bool operator ==(BigNumber a, int b){
        if(a.ToBigInteger() == b) return true;

        return false;
    }

    public static bool operator !=(BigNumber a, int b){
        if(a.ToBigInteger() == b) return false;

        return true;
    }



    public static bool operator <(BigNumber a, BigNumber b){
        if(a.greaterThan(b)!= -1) return false;
        else return true; 
    }

    public static bool operator >(BigNumber a, BigNumber b){
        if(a.greaterThan(b)!= 1) return false;
        else return true; 
    }
    public static bool operator <=(BigNumber a, BigNumber b){
        if(a.greaterThan(b)== 1) return false;
        else return true; 
    }

    public static bool operator >=(BigNumber a, BigNumber b){
        if(a.greaterThan(b)== -1) return false;
        else return true; 
    }
    // -----------------------------------------------------------------------------------------------

}

public static class CustomMath{
    public static int getNumOfSignificantDigits(float f){
        int cnt = 0;
        while(true){
            int temp = (int)f;
            if(f-temp<float.Epsilon){
                break;
            }
            cnt++;
            f *= 10;
        }
        
        return cnt;
    }

    public static int getSignificantDigits(float f){
        for(int i=0;i<getNumOfSignificantDigits(f);i++){
            f *= 10;
        }

        return (int)f;
    }

}

