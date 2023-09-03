using System;
using System.Collections.Generic;

static class MMParser
{
    const string Numbers = "1234567890";

    const char CloseBracket = ']';
    const char OpenBracket  = '[';
    const char Ampersand    = '&';
    const char Greater      = '>';
    const char Dollar       = '$';
    const char Colon        = ':';
    const char Comma        = ',';
    const char At           = '@';

    public static MMConfiguration Parse(string source)
    {
        Dictionary<string, string> auth = new Dictionary<string, string>();

        List<Structure> structures = new List<Structure>();
        List<Trail>     trails     = new List<Trail>();

        char   current = '0';
        string node    = string.Empty;

        for (int index = 0; index < source.Length; ++index)
        {
            current = source[index];

            if (current == Dollar)
            {
                List<int> sequence = new List<int>();

                for (index = index + 1; index < source.Length; ++index)
                {
                    current = source[index];

                    if (Numbers.Contains(current))
                    {
                        node += current;
                        continue;
                    }

                    if (current == Greater || current == Colon)
                    {
                        sequence.Add(int.Parse(node));
                        node = string.Empty;
                        continue;
                    }

                    if (current == Dollar)
                    {
                        int trigger = sequence[0];
                        sequence.RemoveAt(0);

                        trails.Add(new Trail {
                            Trigger  = trigger,
                            Body     = sequence,
                            ms_Decay = int.Parse(node)
                        });

                        node = string.Empty;
                        break;
                    }
                }

                continue;
            }

            if (current == Ampersand)
            {
                List<Segment> segments = new List<Segment>();

                int? trigger = null;
                
                int id    = -1;
                int x     = -1;
                int y     = -1;
                int build = -1;
                int decay = -1;

                bool coordinates       = false;
                bool isParsingNegative = false;

                for (index = index + 1; index < source.Length; ++index)
                {
                    current = source[index];

                    if (current == '-')
                        isParsingNegative = true;
                    
                    if (Numbers.Contains(current))
                    {
                        node += current;
                        continue;
                    }

                    if (current == Greater)
                    {
                        if (trigger is null)
                        {
                            trigger = int.Parse(node);
                            node = string.Empty;
                            continue;
                        }

                        decay = int.Parse(node);

                        segments.Add(new Segment {
                            Id = id,
                            XOffset = x,
                            YOffset = y,
                            ms_Build = build,
                            ms_Decay = decay
                        });

                        node = string.Empty;
                        continue;
                    }

                    if (current == At)
                    {
                        id = int.Parse(node);
                        node = string.Empty;
                        coordinates = true;
                        continue;
                    }

                    if (current == Comma)
                    {
                        if (coordinates)
                        {
                            int value = int.Parse(node);
                            x = isParsingNegative ? -value : value;
                            isParsingNegative = false;
                        }
                        else
                            build = int.Parse(node);

                        node = string.Empty;
                        continue;
                    }

                    if (current == Colon)
                    {
                        int value = int.Parse(node);
                        y = isParsingNegative ? -value : value;
                        isParsingNegative = false;
                        coordinates = false;
                        node = string.Empty;
                        continue;
                    }

                    if (current == Ampersand)
                    {
                        decay = int.Parse(node);

                        segments.Add(new Segment {
                            Id = id,
                            XOffset = x,
                            YOffset = y,
                            ms_Build = build,
                            ms_Decay = decay
                        });

                        structures.Add(new Structure {
                            Trigger = trigger ?? -1,
                            Segments = segments
                        });

                        node = string.Empty;
                        break;
                    }
                }
            }

            if (current == OpenBracket)
            {
                bool isParsingUser        = false;
                bool isParsingPass        = false;
                bool isParsingCredentials = false;

                for (index = index + 1; index < source.Length; ++index)
                {
                    current = source[index];

                    if (Char.IsWhiteSpace(current))
                        continue;

                    if (current == CloseBracket)
                    {
                        if (isParsingUser) auth["email"]    = node;
                        if (isParsingPass) auth["password"] = node;

                        node = string.Empty;
                        break;
                    }

                    node += current;

                    isParsingCredentials = isParsingUser || isParsingPass;

                    if (node == "email" && !isParsingCredentials)
                    {
                        isParsingUser = true;
                        node = string.Empty;
                        continue;
                    }    

                    if (node == "passw" && !isParsingCredentials)
                    {
                        isParsingPass = true;
                        node = string.Empty;
                        continue;
                    }
                }
            }
        }

        return new MMConfiguration
            { Auth = auth, Trails = trails, Structures = structures }; 
    }
}
