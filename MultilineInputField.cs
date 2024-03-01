using UnityEngine;
using TMPro;

public class MultilineInputField : TMP_InputField
{
    protected override void LateUpdate()
    {
        base.LateUpdate();

        // Перехватываем нажатие клавиши Enter
        if (isFocused && Input.GetKeyDown(KeyCode.Return))
        {
            // Добавляем новую строку в текст InputField
            text += "<br>";
            // Устанавливаем курсор в конец текста
            caretPosition = text.Length;
            // Устанавливаем выделение текста в 0, чтобы не выделять новую строку
            selectionAnchorPosition = selectionFocusPosition = caretPosition;
            // Обновляем положение курсора
            MoveTextEnd(false);
        }
    }
}
