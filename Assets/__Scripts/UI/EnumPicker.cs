﻿using System;
using System.Collections;
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
    public bool locked;

    public bool shouldBold = true;
    public Color normalColor = Color.white;
    public Color selectedColor = Color.white;
    public bool resizeSelected = false;
    public float selectedSize = 16;
    private float regularSize;

    private Type type;
    private Dictionary<Enum, TextMeshProUGUI> items = new Dictionary<Enum, TextMeshProUGUI>();
    private TextMeshProUGUI lastSelected;

    public void Initialize(Type type)
    {
        foreach (Enum enumValue in Enum.GetValues(type))
        {
            GameObject option = Instantiate(optionPrefab, optionPrefab.transform.parent);
            TextMeshProUGUI textMesh = option.GetComponent<TextMeshProUGUI>();
            regularSize = textMesh.fontSize;
            textMesh.text = GetDescription(enumValue);
            items.Add(enumValue, textMesh);
            option.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (locked)
                    return;
                Select(textMesh);
                onClick?.Invoke(enumValue);
            });
            option.SetActive(true);
        }
        TextMeshProUGUI defaultSelected = items.First().Value; //todo maybe add an optional default selected parameter
        foreach (TextMeshProUGUI text in items.Values)
        {
            if (shouldBold)
                text.fontStyle &= ~FontStyles.Bold;
            text.color = normalColor;
        }
        Select(defaultSelected);
    }

    private void Select(TextMeshProUGUI toSelect)
    {
        StopAllCoroutines();
        if(lastSelected != null)
        {
            if (shouldBold)
                lastSelected.fontStyle &= ~FontStyles.Bold;
            lastSelected.color = normalColor;
            if (resizeSelected)
                StartCoroutine(InterpolateToSize(lastSelected, regularSize));
        }
        if (shouldBold)
            toSelect.fontStyle |= FontStyles.Bold;
        toSelect.color = selectedColor;
        if (resizeSelected)
            StartCoroutine(InterpolateToSize(toSelect, selectedSize));
        lastSelected = toSelect;
    }

    public void Select(Enum enumValue) => Select(items[enumValue]);

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

    private IEnumerator InterpolateToSize(TextMeshProUGUI textMesh, float size)
    {
        float originalSize = textMesh.fontSize;
        for(int time = 0; time <= 10; time++)
        {
            textMesh.fontSize = Mathf.Lerp(originalSize, size, Mathf.Pow(time / 10f, 1/3f));
            yield return new WaitForSeconds(1 / 60f);
        }
        textMesh.fontSize = size;
    }
}
