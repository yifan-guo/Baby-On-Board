using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogMessenger : MonoBehaviour
{ 
    // Use this to add fields we need
    // form editor view: https://docs.google.com/forms/d/145ztS91_kMN3sWzy7zR2DrIH-k44NQ9BCYPR0rtntLw/edit
    //
    // Use this for entry IDs
    // form taker view: https://docs.google.com/forms/d/e/1FAIpQLSdfI4wbKYRdcLMbhnpP_rc-XL-FacemFqzvsuKKjfKmaR8BdA/viewform?usp=sf_link
    //
    // Use this to view 
    // response spreadsheet: https://docs.google.com/spreadsheets/d/1bw6Xq9ZH3x3sOryXPQnGEQ2ZHnSEufkmUeCE393THpE/edit?resourcekey#gid=8177860

    private const string BASE_URL = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSdfI4wbKYRdcLMbhnpP_rc-XL-FacemFqzvsuKKjfKmaR8BdA/formResponse";

    private bool sent = false;

    public IEnumerator Post(AttemptReport report)
    {
        // <input type="hidden" name="entry.1074061825" value="">
        // <input type="hidden" name="entry.1706035046" value="">
        // <input type="hidden" name="entry.470235340" value="">
        // <input type="hidden" name="entry.1762866937" value="">
        // <input type="hidden" name="entry.1536362842" value="">
        // <input type="hidden" name="entry.1485903512" value="">
        // <input type="hidden" name="entry.169029183" value="">
        // <input type="hidden" name="entry.991958" value="">
        // <input type="hidden" name="entry.1556313048" value="">
        // <input type="hidden" name="entry.21026437" value="">
        // <input type="hidden" name="entry.2083044711" value="">
        // <input type="hidden" name="entry.482325542" value="">
        // <input type="hidden" name="entry.1167282296" value="">
        // <input type="hidden" name="entry.172320985" value="">
        // <input type="hidden" name="entry.789894942" value="">
        // <input type="hidden" name="entry.1691245107" value="">
        // <input type="hidden" name="entry.427089485" value="">
        // <input type="hidden" name="entry.2092209994" value="">
        // <input type="hidden" name="entry.1088033488" value="">
        // <input type="hidden" name="entry.900801655" value="">
        // <input type="hidden" name="entry.1403845558" value="">
        // <input type="hidden" name="entry.79484810" value="">
        // <input type="hidden" name="entry.1872382336" value="">
        // <input type="hidden" name="entry.1317096085" value="">
        // <input type="hidden" name="entry.2028536741" value="">
        // <input type="hidden" name="entry.690325059" value="">
        // <input type="hidden" name="entry.1378110127" value="">
        // <input type="hidden" name="entry.753705781" value="">
        // <input type="hidden" name="entry.655076955" value="">

        yield break;

        // if (sent == true)
        // {
        //     yield break;
        // }

        // Dictionary<string, dynamic> reportDict = report.ToDict();
        
        // WWWForm form = new WWWForm();
        
        // foreach (var item in reportDict)
        // {
        //     form.AddField(
        //         item.Key,
        //         $"{item.Value}");
        // }

        // byte[] rawData = form.data;
        // WWW www = new WWW(
        //     BASE_URL, 
        //     rawData);

        // yield return www;

        // sent = true;
        // www.Dispose();
    }
}