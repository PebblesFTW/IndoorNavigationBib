using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using TMPro;

public class MainScreenState : State {

    private GameObject Panel;
    private TMP_Dropdown Dropdown;
    private TMP_InputField InputField;
    private Button Button;
    private TMP_Text ButtonText;

    private string inputString;
    private string tagName = TagConverter.getTagName(0);
    

    public MainScreenState(StateMachine owner) {
        this.owner = owner;
    }

    public override void StartState() {
        // get references
        Transform panel = this.owner.transform.Find("Canvas").Find("MS Panel");
        this.Panel = panel.gameObject;
        this.Dropdown = panel.Find("Dropdown").gameObject.GetComponent<TMP_Dropdown>();
        this.InputField = panel.Find("Input Standortnummer").gameObject.GetComponent<TMP_InputField>();
        this.Button = panel.Find("Button").gameObject.GetComponent<Button>();
        this.ButtonText = this.Button.GetComponentInChildren<TMP_Text>();

        // add listeners to references
        this.InputField.onValueChanged.AddListener(delegate {OnInputFieldValueChanged(InputField.text); });
        this.Dropdown.onValueChanged.AddListener(delegate {OnDropdownValueChanged(Dropdown.value); });
        this.Button.onClick.AddListener(delegate {OnButtonClick(); });

        this.Panel.SetActive(true);
    }

    public override void UpdateState() {
        if (Application.platform == RuntimePlatform.Android && Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
    }

    public override void FinalizeState() {
        this.Panel.SetActive(false);
        
        // remove listeners
        this.InputField.onValueChanged.RemoveListener(delegate {OnInputFieldValueChanged(InputField.text); });
        this.Dropdown.onValueChanged.RemoveListener(delegate {OnDropdownValueChanged(Dropdown.value); });
        this.Button.onClick.RemoveListener(delegate {OnButtonClick(); });
    }

    public void OnDropdownValueChanged(int option) {
        switch (option) {
            case ((int) TagConverter.EnumTags.Books):
            case ((int) TagConverter.EnumTags.Theses):
                this.InputField.gameObject.SetActive(true);
                this.Button.interactable = false;
                OnInputFieldValueChanged(this.InputField.text);
                break;
            case ((int) TagConverter.EnumTags.Norms):
            case ((int) TagConverter.EnumTags.NewBooks):
            case ((int) TagConverter.EnumTags.BookSell):
            case ((int) TagConverter.EnumTags.LooseLeafCollection):
            case ((int) TagConverter.EnumTags.MagazineCurrent):
            case ((int) TagConverter.EnumTags.MagazineOld):
            case ((int) TagConverter.EnumTags.MagazineSingle):
                this.InputField.gameObject.SetActive(false);
                validEntry();
                this.inputString = TagConverter.getTagName(option);
                break;
            default:
                throw new ArgumentException("Option in Dropdown unknown");
        }
        this.tagName = TagConverter.getTagName(option);
    }

    private void validEntry() {
        this.Button.interactable = true;
        this.ButtonText.color = new Color(0f, 0.6078432f, 0.8666667f, 1f);
        this.ButtonText.text = "Marker einscannen";
    }

    public void OnInputFieldValueChanged(string input) {
        if (input == String.Empty) {
            this.ButtonText.text = "Bitte Regalaufstellung eingeben";
            this.ButtonText.color = new Color(0.3921569f, 0.3921569f, 0.3921569f, 1f);
        }
        else if (Regex.IsMatch(input, @"^[a-zA-Z]{2}\s\d{3,5}((\s[a-zA-Z])|(\s[a-zA-Z]\d{0,3})|(\s[a-zA-Z]\d{0,3}\(\d{1,2}\)))?$")) {
            validEntry();
            this.inputString = input;
        }
        else {
            this.Button.interactable = false;
            this.ButtonText.text = "Ungültige Regalaufstellung";
            this.ButtonText.color = new Color(0.3921569f, 0.3921569f, 0.3921569f, 1f);
        }
    }

/*
    // Regex explanation:

    ^                   // start searching for pattern at the beginning of the string
    [a-zA-Z]{2}         // lower or upper case letter 2 times
    \s                  // whitespace
    \d{3,5}             // 3 to 5 integers
                        // end of initial string - this must be in every booknumber to navigate properly
    (                   // start of supergroup
        (               // start of subgroup 1
            \s          // whitespace
            [a-zA-Z]    // lower or upper case letter
        )               // end of subgroup 1
    |                   // logical or
        (               // start of supgroup 2
            \s          // whitespace    
            [a-zA-Z]    // lower or upper case letter
            \d{0,3}     // 0 to 3 integers
        )               // end of subgroup 2
    |                   // logical or
        (               // start of subgroup 3
            \s          // whtespace
            [a-zA-Z]    // lower or upper case letter
            \d{0,3}     // 0 to 3 integers
            \(          // the character '('
            \d{1,2}     // one to two integers
            \)          // the character ')'
        )               // end of subgroup 3
    )?                  // end of supergroup - ? means 1 or no occurence of the supergroup inside
    $                   // stop searching for pattern at the end of the string

    // The grouping is necessary to accomplish only one or none of the sub groups after the initial string
*/

    public void OnButtonClick() {
        this.owner.navPoint = SearchNavPoint(this.inputString, this.tagName);
        if (this.owner.navPoint == null) {
            ShowNavPointNotFound();
        } else {
            this.Panel.SetActive(false);
            this.owner.ChangeState(new MarkerState(this.owner));
        }
    }

    private GameObject SearchNavPoint(string locationNumber, string tag) {
        if (locationNumber == null || tag == null) {
            throw new ArgumentNullException("location number or tag is null");
        }
        
        if ((tag.Equals(TagConverter.EnumTags.Books.ToString()) || tag.Equals(TagConverter.EnumTags.Theses.ToString()))
                && !Regex.IsMatch(locationNumber,
                @"^[a-zA-Z]{2}\s\d{3,5}((\s[a-zA-Z])|(\s[a-zA-Z]\d{0,3})|(\s[a-zA-Z]\d{0,3}\(\d{1,2}\)))?$")) {
            throw new ArgumentException("location number does not match pattern");
        }

        GameObject[] navPointObjects = GameObject.FindGameObjectsWithTag(tag);

        SortedSet<GameObject> set = new SortedSet<GameObject>(navPointObjects,
                Comparer<GameObject>.Create(
                        (x, y) => new CaseInsensitiveComparer().Compare(x.name, y.name)
                ));

        return set.Where(
                gameobject => new CaseInsensitiveComparer().Compare(gameobject.name, locationNumber) <= 0
        ).LastOrDefault();
    }

    private void ShowNavPointNotFound() {
        this.InputField.text = "";
        this.Button.interactable = false;
        this.ButtonText.text = "Unbekannte Regalaufstellung";
    }
}
