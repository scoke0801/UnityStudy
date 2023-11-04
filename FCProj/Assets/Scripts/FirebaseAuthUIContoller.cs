using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FirebaseAuthUIContoller : MonoBehaviour
{
    public TMP_InputField emailInputField;
    public TMP_InputField passwordInputField;

    public TMP_Text outputText;

    // Start is called before the first frame update
    void Start()
    {
        FirebaseAuthController.Instance.OnChangedLoginState += OnChnagedLoginState;

        FirebaseAuthController.Instance.InitializeFirebase();
    }

    public void CreateUser()
    {
        string email = emailInputField.text;
        string password = passwordInputField.text;


        FirebaseAuthController.Instance.CreateUser(email, password);
    }

    public void SignIn()
    {
        string email = emailInputField.text;
        string password = passwordInputField.text;


        FirebaseAuthController.Instance.SignIn(email, password);
    }

    public void SignOut()
    {
        string email = emailInputField.text;
        string password = passwordInputField.text;


        FirebaseAuthController.Instance.SignOut();
    }

    public void OnChnagedLoginState(bool signedIn)
    {
        outputText.text = signedIn ? "Signed In" : "Signed out";
        outputText.text += FirebaseAuthController.Instance.UserId;
     }

}
