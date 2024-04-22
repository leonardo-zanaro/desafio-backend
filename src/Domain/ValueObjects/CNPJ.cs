using System.Text.RegularExpressions;

namespace Domain.ValueObjects;

public class CNPJ
{
    private readonly string _cnpj;
    public string Value => _cnpj;
    public CNPJ(string cnpj)
    {
        _cnpj = Regex.Replace(cnpj, @"\D", "");

        if (_cnpj.Length != 14 || !IsCnpjValid(_cnpj))
        {
            throw new ArgumentException("Invalid document");
        }
    }

    private bool IsCnpjValid(string cnpj)
    {
        int[] multiplier1 = new int[12] {5,4,3,2,9,8,7,6,5,4,3,2};
        int[] multiplier2 = new int[13] {6,5,4,3,2,9,8,7,6,5,4,3,2};
        int sum;
        int remainder;
        string digit;
        string tempCnpj;
        cnpj = cnpj.Trim();
        cnpj = cnpj.Replace(".", "").Replace("-", "").Replace("/", "");
        if (cnpj.Length != 14)
            return false;
        tempCnpj = cnpj.Substring(0, 12);
        sum = 0;
        for(int i=0; i<12; i++)
            sum += int.Parse(tempCnpj[i].ToString()) * multiplier1[i];
        remainder = (sum % 11);
        if ( remainder < 2)
            remainder = 0;
        else
            remainder = 11 - remainder;
        digit = remainder.ToString();
        tempCnpj = tempCnpj + digit;
        sum = 0;
        for (int i = 0; i < 13; i++)
            sum += int.Parse(tempCnpj[i].ToString()) * multiplier2[i];
        remainder = (sum % 11);
        if (remainder < 2)
            remainder = 0;
        else
            remainder = 11 - remainder;
        digit = digit + remainder.ToString();
        return cnpj.EndsWith(digit);
    }
    
}