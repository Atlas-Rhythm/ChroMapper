﻿using System;
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

    public bool shouldBold = true;
    public Color normalColor = Color.white;
    public Color selectedColor = Color.white;

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
                Select(textMesh);
                onClick?.Invoke(enumValue);
            });
            option.SetActive(true);
        }
        TextMeshProUGUI defaultSelected = items.First(); //todo maybe add an optional default selected parameter
        Select(defaultSelected);
    }

    private void Select(TextMeshProUGUI toSelect)
    {
        foreach (TextMeshProUGUI text in items)
        {
            if (shouldBold)
                text.fontStyle = FontStyles.Normal;
            text.color = normalColor;
        }
        if (shouldBold)
            toSelect.fontStyle = FontStyles.Bold;
        toSelect.color = selectedColor;
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
