using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pixeye.Unity;
//Libererias de incriptación
using System.Text;
using System.Security.Cryptography;

using UnityEngine.Networking;

//librerillas para enviar un email
using System.Net.Mail;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;


//poner una contraseña al script 
public class AppManager : MonoBehaviour
{
    #region PAGE SYSTEM 
    [Foldout("Pages")]public List<GameObject> allPages;
    private int lastPage;

    protected string secretKey = "CampusNET";
    private void Start()
    {
        SetPage(0);

        //print(Md5Sum("Hola mundo" + secretKey));  prueba
    }

    public void SetPage(int _index)
    {
        if (_index != 2)
        {
            lastPage = _index;
        }
            for (int i = 0; i < allPages.Count; i++)
        {
            //sera true o sera false segun la pagina
            allPages[i].SetActive(i == _index);
        }
    }

    public void ReturnPage()
    {
        SetPage(lastPage);
    }

    #endregion

    #region REGISTER
    [Foldout("Register")]public InputField r_username, r_password1, r_password2;

    public void SignUp()
    {
        //condiciones para que sea todo correcto, si no no acepta hacer el Sign Up
        if(r_username.text.Length >= 9 && r_password1.text.Length >= 8 &&
            r_password1.text.Length == r_password2.text.Length)
        {
            StartCoroutine(Register());
            /*print("Este usuario ya existe");
            print("No tienes internet");
            print("Usuario registrado");
            */
        }
        else
        {
            print("Hay algun campo mal");
        }
    }
    IEnumerator Register()
    {
        string hash = Md5Sum(r_username.text + r_password1.text + secretKey);
        WWWForm form = new WWWForm();
        form.AddField("email", r_username.text);
        form.AddField("pass", r_password1.text);
        form.AddField("hash", hash);

        UnityWebRequest request = UnityWebRequest.Post("http://localhost/blues/signup.php", form);
        yield return request.SendWebRequest();

        //print(request.downloadHandler.text);
        string result = request.downloadHandler.text;
        if (result == "true")
        {
            //enviar email al usuario
            SendEmail();
            SetPage(0);
        }
    }

    IEnumerator SendEmail()
    {
        MailMessage mail = new MailMessage();
        mail.From = new MailAddress("mar.prueba@outlook.com");
        mail.To.Add(r_username.text);
        mail.Subject = "Bienvenido a la familia BLUES";
        mail.Body = "Hola\n\nBienvenido/a.";

        SmtpClient server = new SmtpClient("smtp-relay.gmail.com");
        server.Port = 587;
        //dar credenciales al correo
        server.Credentials = new NetworkCredential("mar.prueba@outlook.com", "2d3t1h_5w4q") as ICredentialsByHost;
        server.EnableSsl = true;
        ServicePointManager.ServerCertificateValidationCallback =
            delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            {
                return true;
            };
        server.Send(mail);
        yield return null;


    }

    #endregion

    #region LOGIN
    [Foldout ("Login")]public InputField l_username, l_password;

    public void SignIn()
    {
        //enviar la informacio al PHP en el nube
        StartCoroutine(Login());

    }

    IEnumerator Login()
    {
        string hash = Md5Sum(l_username.text + l_password.text + secretKey);
        WWWForm form = new WWWForm();
        form.AddField("email", l_username.text);
        form.AddField("pass", l_password.text);
        form.AddField("hash", hash);

        UnityWebRequest request = UnityWebRequest.Post("http://localhost/blues/signin.php", form);
        yield return request.SendWebRequest();

        string result = request.downloadHandler.text;
        print(result);
        if(result == "true")
        {
            print("has iniciado sesion correctamente");
            //aqui puedo hacer lo que quiera, porque el usuario ha iniciado session correctamente
            SetPage(3);
        }
        else if(result == "false")
        {
            print("El usuari o la contraseña no existe");
        }
    }

    #endregion

    public string Md5Sum(string strToEncrypt)
    {
        UTF8Encoding ue = new UTF8Encoding();
        byte[] bytes = ue.GetBytes(strToEncrypt);
        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        byte[] hashBytes = md5.ComputeHash(bytes);
        string hashString = "";

        for (int i = 0; i < hashBytes.Length; i++)
        {
            hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
        }
        return hashString.PadLeft(32, '0');
    }

}
