using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnumPicker : MonoBehaviour
{
    public GameObject optionPrefab;
    public event Action<Enum> onClick;

    private Type type;
    private List<TextMeshProUGUI> items = new List<TextMeshProUGUI>();

    public void Initialize(Type type)
    {
        foreach (Enum enumValue in Enum.GetValues(type))
        {
            GameObject option = Instantiate(optionPrefab, optionPrefab.transform.parent);
            TextMeshProUGUI textMesh = option.GetComponent<TextMeshProUGUI>();
            textMesh.text = GetDescription(enumValue);
            items.Add(textMesh);
            option.GetComponent<Button>().onClick.AddListener(() =>
            {
                foreach (TextMeshProUGUI text in items)
                    text.fontStyle = FontStyles.Normal;
                textMesh.fontStyle = FontStyles.Bold;
                onClick?.Invoke(enumValue);
            });
            option.SetActive(true);
        }
        items.First().fontStyle = FontStyles.Bold;
    }

    private static string GetDescription(Enum GenericEnum)
    {
        Type genericEnumType = GenericEnum.GetType();
        MemberInfo[] memberInfo = genericEnumType.GetMember(GenericEnum.ToString());
        if ((memberInfo != null && memberInfo.Length > 0))
        {
            var _Attribs = memberInfo[0].GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
            if ((_Attribs != null && _Attribs.Count() > 0))
            {
                return ((System.ComponentModel.DescriptionAttribute)_Attribs.ElementAt(0)).Description;
            }
        }
        return GenericEnum.ToString();
    }
}
