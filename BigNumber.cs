using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Numerics;
using System;
using System.Linq;

// To control infinite Integer
// Only positibe number

public class BigNumber
{
    private static string[] placeValue = new string[] { "", "만","억", "조", "경", "해", "자", "양", "가", "구", "간" };
    private static int MAX = placeValue.Length;
    private static int TENTHOUSAND = 10000;

    private int[] value = new int[MAX];
    private float fractionPart; // 소수점 아래 파트 

    public static BigNumber Zero = new BigNumber(0);
    // =================================================================================================    
    // Constructor
    public BigNumber()  {   }

    public BigNumber(BigInteger num)
    {
        SetValue(num);
    }

    public BigNumber(int num)
    {
        SetValue(num);
    }

    public BigNumber(float num)
    {
        SetValue(num);
    }

    public BigNumber(string num){
        SetValue(num);
    }

    public BigNumber(double num)
    {
        SetValue((int)num);
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

    public void Init(){
        for(int i=0;i<MAX;i++){
            value[i]=0;
        }
        fractionPart = 0f;
    }
    public void SetValue(BigInteger num)
    {
        Init();
        if(num.ToString().Length > MAX*4) {
            SetMaxValue();
            return;
        }

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

    
    public void SetValue(int num)
    {
        Init();
        int idx =0 ;
        while(num>0){
            value[idx++] = num % TENTHOUSAND;
            num /= TENTHOUSAND;
        }
    }

    public void SetValue(string str)
    {
        Init();
        
        string fraction = "";
        if(str.Contains(".")){
            fraction = str.Substring(str.IndexOf('.'));
            string frPart = CustomMath.getSignificantDigits(float.Parse("0" + fraction));

            this.fractionPart = float.Parse("0" + fraction);
            str = str.Substring(0, str.IndexOf('.'));
        }

        if(str.Length > MAX*4) {
            SetMaxValue();
            return;
        }
        
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

    public void SetValue(BigNumber num)
    {
        Init();
        for(int i=0;i<MAX;i++){
            this.value[i] = num.value[i];
        }
    }


    public void SetValue(float num)
    {
        string numToStr = CustomMath.getSignificantDigits(num);
        int fractionCnt = CustomMath.getNumOfSignificantDigits(num);

        
        this.fractionPart = float.Parse("0." + numToStr.Substring(numToStr.Length - fractionCnt));
        numToStr = numToStr.Substring(0,numToStr.Length - fractionCnt);

        if(numToStr.Length > MAX*4) {
            SetMaxValue();
            return;
        }
        
        int idx = 0;
        while (!numToStr.Equals(""))
        {
            int startIdx = numToStr.Length - 4;
            if (startIdx < 0)
            {
                startIdx = 0;
            }

            string unit = numToStr.Substring(startIdx);
            value[idx++] = int.Parse(unit);
            numToStr = numToStr.Substring(0, numToStr.Length - unit.Length);
        }
    }
    

    public void SetMaxValue(){
        for(int i=0;i<MAX;i++){
            value[i] = 9999;
        }
        return;
    }

    // 단위와 함께 출력 - 큰 단위 값 2개
    public string GetValueWithText()
    {
        if(IsZero()) return "0";

        string res = "";
        int startIdx = getLengthOfValue();

        for (int i = startIdx; i >= 0 && i > startIdx - 2; i--)
        {
            if (value[i] == 0) {
                continue;
            }
     
            res += value[i] + placeValue[i];
        }

        return res;
    }

    // 단위와 함께 출력 - 전부 (테스트용)
    public string GetFullValueWithText()
    {
        if(IsZero()) return "0";

        string res = "";
        int startIdx = getLengthOfValue();

        for (int i = startIdx; i >= 0 && i >= 0 ; i--)
        {
            /*
            if (value[i] == 0) {
                continue;
            }
            */
            res += value[i] + placeValue[i];
        }

        return res;
    }

    // 정수 형태로 반환
    public BigInteger ToBigInteger()
    {
        if(IsZero()) return 0;

        string res = "";
        int startIdx = getLengthOfValue();

        for (int i = startIdx; i >= 0; i--)
        {
            //if(res.Equals("") && i==0){
            //    res += "0";
            //}
            //else
            {
                res += value[i].ToString("D4");
            }  
        }
        return BigInteger.Parse(res);
    }

    // 문자열 형태로 반환
    public override string ToString()
    {
        if(IsZero()) return "0";

        string res = "";

        res = string.Join("", value.Reverse().Select(x=>x.ToString("D4")).ToArray());
        res = res.TrimStart('0');
        if(res.Equals("")) res = "0";

        if(fractionPart > 0f){
            res += '.' + CustomMath.getSignificantDigits(fractionPart);
        }

        return res;
    }

    public int getLengthOfValue(){
        int res = MAX - 1;

        while (res > 0 && value[res] == 0)
        {
            res--;
        }

        return res;
    }
    // =================================================================================================


    // =================================================================================================
    // Status operations
    public bool IsZero()
    {
        foreach(int n in value)
        {
            if (n != 0)
            {
                return false;
            }
        }

        if(fractionPart > 0f) return false;

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
    // 두 수 같으면 0
    // this가 other보다 크면 1
    // this가 other보다 작으면 -1
    private int GreaterThan(BigNumber other)
    {
        for(int i=MAX-1; i>=0;i--){
            if(this.value[i] > other.value[i]) return 1;
            else if(this.value[i] < other.value[i]) return -1;
        }

        if(this.fractionPart > other.fractionPart) return 1;
        else if(this.fractionPart > other.fractionPart) return -1;

        return 0;
    }

    private int GreaterThan(int other)
    {
        if( other < 0 ) return 1;
        else if(other==0){
            if(IsZero()) return 0;
            else return 1;
        }
        
        int cnt = (int)Math.Floor(Math.Log10(other) + 1);
        int startIdx = getLengthOfValue();

        if(startIdx > (cnt-1)/4) return 1;
        else if(startIdx < (cnt-1)/4) return -1;

        int i=startIdx;
        while(i> 0 && other > 0){
            if(this.value[i] > other / TENTHOUSAND) return 1;
            else if(this.value[i] < other / TENTHOUSAND) return -1;

            other -= (other/TENTHOUSAND) * TENTHOUSAND;
            i--;
        }

        if(this.value[i] > other) return 1;
        else if(this.value[i] < other) return -1;
        else return 0;
    }
    // =================================================================================================


    // =================================================================================================
    // operator overloading


    
    // -----------------------------------------------------------------------------------------
    // Arithmetic Operator

    public static BigNumber operator +(BigNumber a, BigNumber b){
        int regroup = 0; // 받아올림

        BigNumber c = new BigNumber(a);

        if(b.fractionPart > 0){
            c.fractionPart += b.fractionPart;
            if(c.fractionPart > 1){
                regroup = 1;
                c.fractionPart -= 1;
            }
        }

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
                c.value[i] -= TENTHOUSAND;
            }
            else
            {
                regroup = 0;
            }
        }

        return c;
    }

    
    public static BigNumber operator +(BigNumber a, int b){
        BigNumber c = new BigNumber(a);
        
        int regroup = 0; // 받아올림
        int i = 0;
        
        while(i < MAX && (b > 0 || regroup > 0)){
            
            if (b%TENTHOUSAND != 0 || regroup != 0)
            {
                c.value[i] += (b % TENTHOUSAND) + regroup;
                if (c.value[i] > 9999)
                {
                    if(i==MAX-1){
                        c.SetMaxValue();
                        break;
                    }
                    regroup = 1;
                    c.value[i] -= TENTHOUSAND;
                }
                else
                {
                    regroup = 0;
                }
            }

            b /= TENTHOUSAND;
            i++;
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

        if(b.fractionPart > 0){
            
            if(c.fractionPart < b.fractionPart){
                c.fractionPart = 1 + c.fractionPart - b.fractionPart;
                regroup = 1;
            }
            else{
                c.fractionPart -= b.fractionPart;
            }
        }

        for (int i = 0; i < MAX; i++)
        {
            if (b.value[i] == 0 && regroup == 0)
            {
                continue;
            }

            if (c.value[i] < b.value[i])
            {
                c.value[i] = c.value[i] + TENTHOUSAND - b.value[i] - regroup;
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
        // if minuend is zero
        if (a==0) return a;

        BigNumber c = new BigNumber(a);
        
        int regroup = 0; // 받아내림
        int idx = 0;

        // if minuend is less than subtrahend
        if(c < b){
            for(int i = 0; i < MAX; i++)
            {
                c.value[i] = 0;
            }
            return c;
        }

        while(idx < MAX && (b > 0 || regroup>0)){
             int temp = b % TENTHOUSAND;
             if(temp != 0 || regroup != 0){
                 if(temp > c.value[idx]){
                     c.value[idx] = c.value[idx] + TENTHOUSAND - temp - regroup;
                     regroup = 1;
                 }
                 else{
                     c.value[idx] = c.value[idx] - temp - regroup;
                     regroup = 0;
                 }
             }

             b /= TENTHOUSAND;
             idx++;
        }
        return c;
    }

/*
    public static BigNumber operator *(BigNumber a, int b){
        if(b < 0) return null;

        BigNumber c = new BigNumber(0);

        if(b==0) return c;

        List<int> listB = new List<int>(); // b를 네자리씩 분할
        while(b>0){
            listB.Add(b%TENTHOUSAND);
            b/=TENTHOUSAND;
        }
        int endIdx = a.getLengthOfValue();

        for(int idxOfListB=0;idxOfListB<listB.Count;idxOfListB++){
            int partB = listB[idxOfListB];

            BigNumber tempRes = new BigNumber(0);
            int regroup = 0; // 받아올림
            int i = 0;
            
            while(i <= endIdx || regroup > 0){
                if(i+idxOfListB >= MAX){
                    tempRes.SetMaxValue();
                    return tempRes;
                }

                tempRes.value[i+idxOfListB] = a.value[i]*partB + regroup;
                if (tempRes.value[i+idxOfListB] > 9999)
                {
                    if(i+idxOfListB==MAX-1){
                        tempRes.SetMaxValue();
                        return tempRes;
                    }
                    regroup = tempRes.value[i+idxOfListB] / TENTHOUSAND;
                    tempRes.value[i+idxOfListB] %= TENTHOUSAND;
                }
                else
                {
                    regroup = 0;
                }   
                i++;
            }
            c += tempRes;
        }

        return c;
    }
*/

    public static BigNumber operator *(BigNumber a, float b){
        if(b < 0) return null;

        if(Mathf.Abs(b)<float.Epsilon) return Zero;
        
        List<int> listB = new List<int>(); // b를 네자리씩 분할
        List<int> listA = new List<int>(); // a의 유효숫자를 네자리씩 분할
        string bToStr = CustomMath.getSignificantDigits(b);

        // a 유효숫자 잘라넣기 
        if(!(Mathf.Abs(a.fractionPart)<float.Epsilon)){
            // BigNumber a에 소수점 값이 있는 경우
            string aToStr = a.ToString().Remove(a.ToString().IndexOf("."),1);
            while(aToStr.Length > 0){
                int start = aToStr.Length - 4;
                if (start < 0)
                {
                    start = 0;
                }

                string unit = aToStr.Substring(start);
                listA.Add(int.Parse(unit));
                aToStr = aToStr.Substring(0, aToStr.Length - unit.Length);
            }
        }
        else{
            foreach(int v in a.value){
                listA.Add(v);
            }
        }

        // b 유효숫자 잘라넣기 
        while(bToStr.Length > 0){
            int start = bToStr.Length - 4;
            if (start < 0)
            {
                start = 0;
            }

            string unit = bToStr.Substring(start);
            listB.Add(int.Parse(unit));
            bToStr = bToStr.Substring(0, bToStr.Length - unit.Length);
        }

        int endIdx = listA.Count;
        int tempArrCnt = listB.Count + listA.Count;
        int[] res  = new int[tempArrCnt];
 

        // 곱하기 
        for(int idxOfListB=0;idxOfListB<listB.Count;idxOfListB++){
            int partB = listB[idxOfListB];

            int[] tempRes = new int[tempArrCnt];
            int regroup = 0; // 받아올림
            int idxOfListA = 0;
            
            while(idxOfListA < listA.Count){
                tempRes[idxOfListA+idxOfListB] = listA[idxOfListA]*partB + regroup;
                
                if (tempRes[idxOfListA+idxOfListB] > 9999)
                {
                    regroup = tempRes[idxOfListA+idxOfListB] / TENTHOUSAND; 
                    tempRes[idxOfListA+idxOfListB] %= TENTHOUSAND;
                }
                else
                {
                    regroup = 0;
                }   
                idxOfListA++;
            }

            if(regroup!=0){
                tempRes[idxOfListA+idxOfListB] += regroup;
            }

            regroup = 0;
            for(int j=0;j<tempArrCnt;j++){
                res[j] += tempRes[j] + regroup;
                if(res[j]>9999){
                    regroup = res[j]/TENTHOUSAND;
                    res[j] %=TENTHOUSAND;
                }
                else{
                    regroup = 0;
                }
            }
        }
        

        string resToStr = "";
        
        Array.Reverse(res);
        resToStr = string.Join("",res.Select(x=>x.ToString("D4")).ToArray());        
        resToStr = resToStr.TrimStart('0');

        if(resToStr.Equals("")) {
            return Zero;
        }

        BigNumber c = new BigNumber();

        int numOfFractionB = CustomMath.getNumOfSignificantDigits(b);
        int numOfFractionA = CustomMath.getNumOfSignificantDigits(a.fractionPart);
        if(resToStr.Length > (numOfFractionB+numOfFractionA)){
            resToStr = resToStr.Insert(resToStr.Length - (numOfFractionB+numOfFractionA),".");
        }
        else{
            resToStr = "0." + resToStr;
        }
        c.SetValue(resToStr);

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
        if(a.GreaterThan(b) == 0) return true;

        return false;
    }

    public static bool operator !=(BigNumber a, int b){
        if(a.GreaterThan(b) == 0) return false;

        return true;
    }



    public static bool operator <(BigNumber a, BigNumber b){
        if(a.GreaterThan(b)!= -1) return false;
        else return true; 
    }

    public static bool operator <(BigNumber a, int b){
        if(a.GreaterThan(b)!= -1) return false;
        else return true; 
    }

    public static bool operator >(BigNumber a, BigNumber b){
        if(a.GreaterThan(b)!= 1) return false;
        else return true; 
    }

    public static bool operator >(BigNumber a, int b){
        if(a.GreaterThan(b)!= 1) return false;
        else return true; 
    }

    public static bool operator <=(BigNumber a, BigNumber b){
        if(a.GreaterThan(b)== 1) return false;
        else return true; 
    }

    
    public static bool operator <=(BigNumber a, int b){
        if(a.GreaterThan(b)== 1) return false;
        else return true; 
    }


    public static bool operator >=(BigNumber a, BigNumber b){
        if(a.GreaterThan(b)== -1) return false;
        else return true; 
    }

    public static bool operator >=(BigNumber a, int b){
        if(a.GreaterThan(b)== -1) return false;
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

    
    public static int getNumOfSignificantDigits(double d){
        int cnt = 0;
        while(true){
            int temp = (int)d;
            if(d-temp<double.Epsilon){
                break;
            }
            cnt++;
            d *= 10;
        }
        
        return cnt;
    }

    public static string getSignificantDigits(float f){
        string res ="";

        if(Mathf.Abs(f)<float.Epsilon) return res;
        int loopCnt = getNumOfSignificantDigits(f);

        for(int i=0;i<loopCnt;i++){
            f *= 10;
        }

        res = ((int)f).ToString("G");
        return res;
    }


    public static string getSignificantDigits(double d){
        string res ="";

        if(Math.Abs(d)<double.Epsilon) return res;
        int loopCnt = getNumOfSignificantDigits(d);

        for(int i=0;i<loopCnt;i++){
            d *= 10;
        }

        res = ((int)d).ToString("G");
        return res;
    }
}
