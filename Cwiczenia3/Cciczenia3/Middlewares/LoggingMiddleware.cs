using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    public LoggingMiddleware(RequestDelegate next) { _next = next; }
    public async Task InvokeAsync(HttpContext httpContext)
    {
        string path = @"C:\Users\Public\requestsLog.txt";
        string wszystko = "";
        httpContext.Request.EnableBuffering();
        //Log log = new Log();
        string metodaHTTP = httpContext.Request.Method;
        wszystko += DateTime.Now.ToString()+"\n"+"Metoda: " + metodaHTTP + "\n";
        string sciezkaHTTP = httpContext.Request.Path;
        wszystko += "Sciezka: " + sciezkaHTTP + "\n";
        string bodyStr = "";

        using(StreamReader reader = new StreamReader(httpContext.Request.Body,Encoding.UTF8, true, 1024, true))
        {
            bodyStr = await reader.ReadToEndAsync();
            httpContext.Request.Body.Position = 0;
        }

        wszystko += "Bddy: " + bodyStr + "\n";
        var querryStr = httpContext.Request.QueryString.ToString();
        wszystko += "Query: " + querryStr.ToString() + "\n";
        /*if (!System.IO.File.Exists(path))
        {
            System.IO.File.WriteAllText(path, all);

        }
        //File.WriteAllText(path, all);*/
     
        if (!System.IO.File.Exists(path))
        {
            FileStream plik = new FileStream(path, FileMode.Create, FileAccess.Write);
            StreamWriter zapis = new StreamWriter(plik);
            zapis.Write(wszystko);
            zapis.Close();
            plik.Close();
            
        }
        else
        {
            FileStream plik = new FileStream(path, FileMode.Open, FileAccess.Write);
            StreamWriter zapis = new StreamWriter(plik);
            zapis.Write(wszystko);
            zapis.Close();
            plik.Close();
        }      
        
        await _next(httpContext);


    }
}
