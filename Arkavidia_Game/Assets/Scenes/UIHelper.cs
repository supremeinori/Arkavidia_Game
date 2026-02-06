using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Unity.Cinemachine.Samples
{
    [RequireComponent(typeof(UIDocument))]
    public class UIHelperNT : MonoBehaviour
    {
        public bool VisibleAtStart = true;

        public string HelpTitle;

        [TextArea(minLines: 10, maxLines: 50)]
        public string HelpText;

        public KeyCode toggleKey = KeyCode.E;

        [Tooltip("Event sent when the help window is dismissed")]
        public UnityEvent OnHelpDismissed = new();

        VisualElement m_HelpBox;
        Button m_HelpButton, m_CloseButton;

        bool isVisible;

        void OnEnable()
        {
            var uiDocument = GetComponent<UIDocument>();

            // Toggle Button (optional)
            m_HelpButton = uiDocument.rootVisualElement.Q<Button>("HelpButton");
            if (m_HelpButton != null)
                m_HelpButton.RegisterCallback<ClickEvent>(OpenHelpBox);

            // Help Window
            m_HelpBox = uiDocument.rootVisualElement.Q("HelpTextBox");

            if (uiDocument.rootVisualElement.Q<Label>("HelpTextBox__Title") is Label helpTitle)
                helpTitle.text = string.IsNullOrEmpty(HelpTitle)
                    ? SceneManager.GetActiveScene().name
                    : HelpTitle;

            if (uiDocument.rootVisualElement.Q<Label>("HelpTextBox__ScrollView__Label") is Label helpLabel)
                helpLabel.text = HelpText;

            // Close Button
            m_CloseButton = uiDocument.rootVisualElement.Q<Button>("HelpTextBox__CloseButton");
            if (m_CloseButton != null)
                m_CloseButton.RegisterCallback<ClickEvent>(CloseHelpBox);

            if (VisibleAtStart)
                Show();
            else
                Hide();
        }

        void Update()
        {
            if (Input.GetKeyDown(toggleKey))
            {
                if (isVisible)
                    Hide();
                else
                    Show();
            }
        }

        void OnDisable()
        {
            Hide();

            if (m_HelpButton != null)
                m_HelpButton.UnregisterCallback<ClickEvent>(OpenHelpBox);

            if (m_CloseButton != null)
                m_CloseButton.UnregisterCallback<ClickEvent>(CloseHelpBox);
        }

        // =====================
        void Show()
        {
            if (m_HelpButton != null)
                m_HelpButton.visible = false;

            if (m_HelpBox != null)
                m_HelpBox.visible = true;

            isVisible = true;
        }

        void Hide()
        {
            if (m_HelpButton != null)
                m_HelpButton.visible = true;

            if (m_HelpBox != null)
                m_HelpBox.visible = false;

            isVisible = false;
        }

        void OpenHelpBox(ClickEvent click)
        {
            Show();
        }

        void CloseHelpBox(ClickEvent click)
        {
            Hide();
            VisibleAtStart = false;
            OnHelpDismissed.Invoke();
        }
    }
}
