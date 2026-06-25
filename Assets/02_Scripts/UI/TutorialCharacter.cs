using UnityEngine;

public class TutorialCharacter : MonoBehaviour
{
    [SerializeField] private CharacterSlot characterSlot;
    [SerializeField] private TutorialDialogueUI dialogueUI;

    public void Setup(Character character)
    {
        characterSlot.SetCharacter(character, character.characterImage);
    }

    public void Say(string text)
    {
        dialogueUI.Show(characterSlot.CurrentCharacter.characterName, text);
    }

    public void SetExpression(FaceType face, EyebrowType eyebrow = EyebrowType.None, bool useScale = true)
    {
        ExpressionData data = new ExpressionData()
        {
            face = face,
            eyebrow = eyebrow,
            useDOScale = useScale
        };

        characterSlot.SetExpression(data);
    }

    public void PlayEmotion(Emotion emotion)
    {
        Sprite icon = characterSlot.CurrentCharacter.emotionIcons.Find(x => x.emotion == emotion)?.icon;

        if (icon != null)
            characterSlot.PlayEmotionIcon(emotion);
    }
}
