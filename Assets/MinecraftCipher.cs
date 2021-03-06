﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KModkit;
using System.Linq;
using System.Text.RegularExpressions;
using System;
using rnd=UnityEngine.Random;

public class MinecraftCipher : MonoBehaviour
{
    public KMBombInfo Bomb;
    public KMSelectable[] Button;
    public KMSelectable clear, submit;
    public TextMesh Message_Display, Input_Display;
    public TextMesh[] button_label;
    public KMAudio Audio;
    static int _moduleIdCounter = 1;
    int _moduleId = 0;

    
    string input = "";
    string answer;
    string[] decrypted_array = new string[36]
    {
        "SPARKLEZ",
        "DIAMOND",
        "OBSIDIAN",
        "SLAYER",
        "SNIPER",
        "MINER",
        "ENGINEER",
        "REPEATER",
        "SUGAR",
        "LAPIS",
        "WART",
        "GUNPOWDER",
        "DRAGON",
        "MELON",
        "SPIDER",
        "MAGMA",
        "CARROT",
        "BLAZE",
        "COBBLESTONE",
        "FARMER",
        "RABBIT",
        "BREATH",
        "PUFFERFUSH",
        "TEAR",
        "ARMOR",
        "PHANTOM",
        "MILLION",
        "HYPIXEL",
        "TOURNAMENT",
        "CRAFTING",
        "ENDING",
        "PEARL",
        "CROPS",
        "DROWNED",
        "LIBRARY",
        "NETHERITE",
    };

    string[] return_message = new string[36]
    {
        "CREEPER",
        "PICKAXE",
        "NETHER",
        "SHARPNESS",
        "POWER",
        "EFFICIENCY",
        "REDSTONE",
        "AUTOCLICK",
        "SPEED",
        "EXPERIENCE",
        "AWKWARD",
        "SPLASH",
        "BREATH",
        "HEALTH",
        "POISON",
        "FIRE",
        "VISION",
        "STRENGTH",
        "COMPACTOR",
        "UNBREAKING",
        "JUMP",
        "LINGERING",
        "BREATHING",
        "REGENERATION",
        "RESISTANCE",
        "SLOWFALL",
        "MIDAS",
        "FIGHTING",
        "MONDAY",
        "MODULE",
        "EGG",
        "TELEPORT",
        "BREAD",
        "TRIDENT",
        "SILVERFISH",
        "UNINSTALL",
    };

    char[] alphabets_exist = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
    string chosenword;
    int getAlphabeticPosition(char c)
    {
        return "ABCDEFGHIJKLMNOPQRSTUVWXYZ".IndexOf(c) + 1;
    }

    char getAlphabeticPosition(int x)
    {
        while (x > 25)
            x -= 26;
        while (x < 0)
            x += 26;
        return "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ElementAt(x);
    }

    void Awake()
    {
        _moduleId = _moduleIdCounter++;
        clear.OnInteract += delegate ()
        {
            clear_input();
            return false;
        };
        submit.OnInteract += delegate ()
        {
            checkAns();
            return false;
        };
        for (int i=0;i<26;i++)
        {
            int j = i;
            Button[i].OnInteract += delegate ()
            {
                handle_input(j);
                return false;
            };
        }
    }

    void Start ()
    {
        init();
	}

    void init()
    {
        randomization();
        clear_input();
        generate_message();
    }

    public int digital_root(int i)
    {
        int j = i % 10 + i / 10;
        int k = j % 10 + j / 10;
        return k;
    }

    void randomization()
    { 
        
        for (int i = 0; i < 100; i++)
        {
            char temp;
            int a = rnd.Range(0, 26);
            int b;
            do
                b = rnd.Range(0, 26);
            while (a == b);
            temp=alphabets_exist[a];
            alphabets_exist[a] = alphabets_exist[b];
            alphabets_exist[b] = temp;
        }
        string randomized_string_logreturn = new string(alphabets_exist);
        for (int i = 0; i <= 25; i++)
            button_label[i].text = randomized_string_logreturn[i].ToString();
        Debug.LogFormat("[Minecraft Cipher #{0}]Randomized keyboard in reading order:{1}", _moduleId, randomized_string_logreturn);
    }


    void generate_message()
    {
        int chosenword_number = rnd.Range(0, 36);
        chosenword = decrypted_array[chosenword_number];
        char[] chosenwordarray = chosenword.ToCharArray();
        int len = chosenword.Length;
        int[] obtained=new int[len];
        char[] encrypted_message_chararray = new char[len];
        var serialdigits = Bomb.GetSerialNumberNumbers();
        int min = 10;
        int max = 0;
        foreach (int digit in serialdigits)
        {
            if (digit < min)
                min = digit;
            if (digit > max)
                max = digit;
        }
        if (Bomb.IsIndicatorOn(Indicator.IND))
        {
            obtained[0] = -3;
            Debug.LogFormat("[Minecraft Cipher #{0}]First letter: rule 1",_moduleId);

        }
        else if(Bomb.GetBatteryCount() == 0)
        {
            obtained[0] = -2;
            Debug.LogFormat("[Minecraft Cipher #{0}]First letter: rule 2",_moduleId);
        }
        else if(Bomb.GetIndicators().Count() == 0)
        {
            obtained[0] = +3;
            Debug.LogFormat("[Minecraft Cipher #{0}]First letter: rule 3",_moduleId);
        }
        else
        {
            int craftingtablecount = 0;
            foreach (var module in Bomb.GetModuleIDs())
            {
                if (module == "needycrafting")
                {
                    craftingtablecount++;
                    break;
                }
            }
            if (craftingtablecount > 0)
            {
                obtained[0] = +2;
                Debug.LogFormat("[Minecraft Cipher #{0}]First letter: rule 4", _moduleId);
            }
            else
            {
                obtained[0] = +Bomb.GetPortCount();
                Debug.LogFormat("[Minecraft Cipher #{0}]First letter: rule 5",_moduleId);
            }
        }
        encrypted_message_chararray[0] = getAlphabeticPosition(getAlphabeticPosition(chosenwordarray[0]) - obtained[0] - 1);
        Debug.LogFormat("[Minecraft Cipher #{0}]Chosen word: {1}", _moduleId,chosenword);
        Debug.LogFormat("[Minecraft Cipher #{0}]Letter 1: {1},{2}",_moduleId, encrypted_message_chararray[0],obtained[0]);
        if ((Bomb.GetBatteryHolderCount() == 4) && (Bomb.GetPortCount(Port.Parallel) > 0))
        {
            obtained[1] = +4;
            Debug.LogFormat("[Minecraft Cipher #{0}]Second letter: rule 1",_moduleId);
        }
        else if (getAlphabeticPosition(chosenwordarray[0]) % 3 == 0)
        { 
            obtained[1] = +2;
            Debug.LogFormat("[Minecraft Cipher #{0}]Second letter: rule 2",_moduleId);
        }
        else
        {
            obtained[1] = -min;
            Debug.LogFormat("[Minecraft Cipher #{0}]Second letter: rule 3",_moduleId);
        }
        encrypted_message_chararray[1] = getAlphabeticPosition(getAlphabeticPosition(chosenwordarray[1]) - obtained[1] - 1);
        Debug.LogFormat("[Minecraft Cipher #{0}]Letter 2:{1},{2}", _moduleId,encrypted_message_chararray[1], obtained[1]);
        for (int i = 2; i < len; i++)
        {
            if (obtained[i - 1] == 0 || obtained[i - 2] == 0)
            {
                obtained[i] = Math.Abs(obtained[i - 1] + obtained[i - 2]) - 10;
                Debug.LogFormat("[Minecraft Cipher #{0}]Letter {1}: rule 1", _moduleId,i + 1);
            }
            else if (obtained[i - 1] < 0 && obtained[i - 2] < 0)
            {
                obtained[i] = Math.Abs(obtained[i - 1] - obtained[i - 2]);
                Debug.LogFormat("[Minecraft Cipher #{0}]Letter {1}: rule 2", _moduleId,i + 1);
            }
            else if (getAlphabeticPosition(chosenwordarray[i - 1]) % 2 == 1 && (getAlphabeticPosition(chosenwordarray[i - 2]) % 2 == 1))
            {
                obtained[i] = max - getAlphabeticPosition(chosenwordarray[i - 2]);
                Debug.LogFormat("[Minecraft Cipher #{0}]Letter {1}: rule 3", _moduleId,i + 1);
            }
            else if (getAlphabeticPosition(chosenwordarray[i - 1]) % 2 == 0 && (getAlphabeticPosition(chosenwordarray[i - 2]) % 2 == 0))
            {
                obtained[i] = obtained[i - 1] + obtained[i - 2];
                Debug.LogFormat("[Minecraft Cipher #{0}]Letter {1}: rule 4",_moduleId, i + 1);
            }
            else
            {
                obtained[i] = getAlphabeticPosition(chosenwordarray[i - 1]);
                Debug.LogFormat("[Minecraft Cipher #{0}]Letter {1}: rule 5(otherwise)", _moduleId,i + 1);
            }
            encrypted_message_chararray[i] = getAlphabeticPosition(getAlphabeticPosition(chosenwordarray[i]) - obtained[i] - 1);
            Debug.LogFormat("[Minecraft Cipher #{0}]Letter {1}:{2},{3}", _moduleId, i + 1, encrypted_message_chararray[i], obtained[i]);
        }
        string encrypted_message = new string(encrypted_message_chararray);
        Message_Display.text=encrypted_message;
        string decrypted_return = return_message[chosenword_number];
        Debug.LogFormat("[Minecraft Cipher #{0}]Return word: {1}",_moduleId,decrypted_return);
        bool[] valid = new bool[decrypted_return.Length];
        int[] decrypted_return_value = new int[decrypted_return.Length];
        char[] answer_array = new char[decrypted_return.Length];
        for (int i=0;i< decrypted_return.Length;i++)
        {
           decrypted_return_value[i] = getAlphabeticPosition(decrypted_return[i]);
            if(i==0)
            {
                valid[i] = true;

            }
            else if(i==1)
            {
                if (digital_root(decrypted_return_value[i]) == decrypted_return_value[i] % 10 || digital_root(decrypted_return_value[i]) == decrypted_return_value[i] / 10)
                    valid[i] = true;
                else valid[i] = false;
                
            }
            else
            {
                if (valid[i - 1] == true && valid[i - 2] == true)
                    valid[i] = false;
                else if (valid[i - 1] == false && valid[i - 2] == false)
                    valid[i] = true;
                else if (digital_root(decrypted_return_value[i]) == (decrypted_return_value[i] % 10) || digital_root(decrypted_return_value[i]) == (decrypted_return_value[i] - decrypted_return_value[i] % 10) / 10)
                    valid[i] = true;
                else valid[i] = false;
            }
            if (valid[i] == true)
            {
                Debug.LogFormat("[Minecraft Cipher #{0}]Letter {1} is valid, digital root is {2}.", _moduleId, i + 1, digital_root(decrypted_return_value[i]));
                switch (digital_root(decrypted_return_value[i]))
                {
                    case 1:
                        decrypted_return_value[i] += Bomb.GetSerialNumberNumbers().Count();
                        break;
                    case 2:
                        decrypted_return_value[i] += Bomb.GetSerialNumberLetters().Count();
                        break;
                    case 3:
                        decrypted_return_value[i] += Bomb.GetModuleNames().Count();
                        break;
                    case 4:
                        decrypted_return_value[i] = 27 - decrypted_return_value[i];
                        break;
                    case 5:
                        decrypted_return_value[i] -= Bomb.GetIndicators().Count();
                        break;
                    case 6:
                        decrypted_return_value[i] += digital_root(decrypted_return_value[i]);
                        break;
                    case 7:
                        decrypted_return_value[i] -= Bomb.GetPortCount();
                        break;
                    case 8:
                        decrypted_return_value[i] -= Bomb.GetStrikes();
                        break;
                    case 9:
                        decrypted_return_value[i] += Bomb.GetBatteryHolderCount();
                        break;
                }
            }
            else Debug.LogFormat("[Minecraft Cipher #{0}]Letter {1} is invalid, digital root is {2}.", _moduleId, i + 1, digital_root(decrypted_return_value[i]));
            answer_array[i] = getAlphabeticPosition(decrypted_return_value[i]-1);
        }
        answer = new string(answer_array);
        Debug.LogFormat("[Minecraft Cipher #{0}]Answer: {1}", _moduleId, answer);
    }

    void clear_input()
    {
        input = "";
        Input_Display.text = input;
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, clear.transform);
    }

    void handle_input(int i)
    {
        input += alphabets_exist.ElementAt(i).ToString();
        Input_Display.text = input;
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Button[i].transform);
    }

    void checkAns()
    {
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, submit.transform);
        Debug.LogFormat("[Minecraft Cipher #{0}]Input: {1}", _moduleId, input);
        if(input==answer)
        {
            GetComponent<KMBombModule>().HandlePass();
            Debug.LogFormat("[Minecraft Cipher #{0}]Correct. Module disarmed.",_moduleId);
            Audio.PlaySoundAtTransform("Sweden", transform);
        }
        else
        {
            GetComponent<KMBombModule>().HandleStrike();
            Debug.LogFormat("[Minecraft Cipher #{0}]Incorrect. Strike incurred.",_moduleId);
            init();
        }
    }

#pragma warning disable 414
    private string TwitchHelpMessage = "!{0} 1 2 3 4 5 6 [Press button of that position, space are required, no letters allowed] | !{0} clear [Clears input] | !{0} submit [Submit input] | You cannnot do !{0} submit (answer)";
#pragma warning restore 414

    IEnumerator ProcessTwitchCommand(string command)
    {
        command = command.ToLowerInvariant().Trim();
        if (command.Equals("clear"))
        {
            yield return null;
            yield return new[] { clear };
        }
        else if (command.Equals("submit"))
        {
            yield return null;
            yield return new[] { submit };
        }
        else
        {
            var numbers = new List<int>();
            string[] parameters = command.Split(' ');
            foreach(var s in parameters)
            {
                int i;
                if (!int.TryParse(s.ToString(), out i) || i>26 || i<1)
                    yield break;
                numbers.Add(i);
            }
            foreach(int i in numbers)
            {
                yield return null;
                Button[i - 1].OnInteract();
                yield return new WaitForSeconds(0.125f);
            }
        }
        yield return null;
    }
}