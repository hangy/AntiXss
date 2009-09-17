// ***************************************************************
// <copyright file="CssParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.TextConverters.Internal.Css
{
    using System;
    using System.IO;
    using System.Text;
    using System.Globalization;
    using Microsoft.Exchange.Data.Internal;
    using Microsoft.Exchange.Data.TextConverters.Internal.Text;
    using Microsoft.Exchange.Data.TextConverters.Internal.Html;

    

    internal enum CssParseMode
    {
        StyleAttribute,
        StyleTag,
        External
    }

    
    

    internal class CssParser : IDisposable
    {
        

        private ConverterInput input;
        private bool endOfFile;

        private CssParseMode parseMode = CssParseMode.StyleAttribute;
        private bool isInvalid;

        private char[] parseBuffer;
        private int parseStart;
        private int parseCurrent;
        private int parseEnd;

        private int ruleDepth;

        
        protected CssTokenBuilder tokenBuilder;
        private CssToken token;

        private static readonly string[] SafeTermFunctions = { "rgb", "counter" };
        private static readonly string[] SafePseudoFunctions = { "lang" };

        internal const int MaxCssLength = 512 * 1024;

        
        
        public CssParser(ConverterInput input, int maxRuns, bool testBoundaryConditions)
        {
            this.input = input;

            this.tokenBuilder = new CssTokenBuilder(null, 256, 256, maxRuns, testBoundaryConditions);

            this.token = this.tokenBuilder.Token;
        }

        
        
        public CssToken Token { get { return this.token; } }

        
        
        void IDisposable.Dispose()
        {
            if (this.input != null /*&& this.input is IDisposable*/)
            {
                ((IDisposable)this.input).Dispose();
            }

            this.input = null;
            this.parseBuffer = null;
            this.token = null;

            GC.SuppressFinalize(this);
        }

        
        
        public void Reset()
        {
            this.endOfFile = false;

            this.parseBuffer = null;
            this.parseStart = 0;
            this.parseCurrent = 0;
            this.parseEnd = 0;
            this.ruleDepth = 0;
        }

        
        
        public void SetParseMode(CssParseMode parseMode)
        {
            this.parseMode = parseMode;
        }

        
        
        
        public CssTokenId Parse()
        {
            if (this.endOfFile)
            {
                return CssTokenId.EndOfFile;
            }

            char ch;
            CharClass charClass;
            this.tokenBuilder.Reset();

            InternalDebug.Assert(this.parseCurrent >= this.parseStart);

            char[] parseBuffer = this.parseBuffer;
            int parseCurrent = this.parseCurrent;
            int parseEnd = this.parseEnd;

            if (parseCurrent >= parseEnd)
            {
                bool readMore = this.input.ReadMore(ref this.parseBuffer, ref this.parseStart, ref this.parseCurrent, ref this.parseEnd);
                InternalDebug.Assert(readMore);

                if (this.parseEnd == 0)
                {
                    
                    return CssTokenId.EndOfFile;
                }

                
                

                this.tokenBuilder.BufferChanged(this.parseBuffer, this.parseStart);

                parseBuffer = this.parseBuffer;
                parseCurrent = this.parseCurrent;
                parseEnd = this.parseEnd;
            }

            InternalDebug.Assert(parseCurrent < parseEnd);

            
            ch = parseBuffer[parseCurrent];
            charClass = ParseSupport.GetCharClass(ch);

            int start = parseCurrent;

            if (this.parseMode == CssParseMode.StyleTag)
            {
                this.ScanStyleSheet(ch, ref charClass, ref parseCurrent);
                if (start >= parseCurrent)
                {
                    
                    InternalDebug.Assert(false);

                    
                    this.tokenBuilder.Reset();

                    return CssTokenId.EndOfFile;
                }

                if (this.tokenBuilder.Incomplete)
                {
                    this.tokenBuilder.EndRuleSet();
                }
            }
            else
            {
                InternalDebug.Assert(this.parseMode == CssParseMode.StyleAttribute);

                this.ScanDeclarations(ch, ref charClass, ref parseCurrent);
                if (parseCurrent < parseEnd)
                {
                    
                    this.endOfFile = true;

                    this.tokenBuilder.Reset();
                    return CssTokenId.EndOfFile;
                }

                if (this.tokenBuilder.Incomplete)
                {
                    this.tokenBuilder.EndDeclarations();
                }
            }

            this.endOfFile = (parseCurrent == parseEnd);

            this.parseCurrent = parseCurrent;
            return this.token.TokenId;
        }

        
        private char ScanStyleSheet(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            int parseEnd = this.parseEnd;
            char[] parseBuffer = this.parseBuffer;
            int start;

            do
            {
                start = parseCurrent;

                ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
                if (parseCurrent == parseEnd)
                {
                    return ch;
                }

                if (this.IsNameStartCharacter(ch, charClass, parseCurrent) || ch == '*' ||
                    ch == '.' || ch == ':' || ch == '#' || ch == '[')
                {
                    ch = this.ScanRuleSet(ch, ref charClass, ref parseCurrent);
                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }

                    if (!this.isInvalid)
                    {
                        
                        return ch;
                    }
                }
                else if (ch == '@')
                {
                    ch = this.ScanAtRule(ch, ref charClass, ref parseCurrent);
                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }

                    if (!this.isInvalid)
                    {
                        
                        return ch;
                    }
                }
                else if (ch == '/' && parseCurrent < parseEnd && parseBuffer[parseCurrent + 1] == '*')
                {
                    ch = this.ScanComment(ch, ref charClass, ref parseCurrent);
                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }
                }
                else if (ch == '<')
                {
                    ch = this.ScanCdo(ch, ref charClass, ref parseCurrent);
                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }
                }
                else if (ch == '-')
                {
                    ch = this.ScanCdc(ch, ref charClass, ref parseCurrent);
                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }
                }
                else
                {
                    this.isInvalid = true;
                }

                if (this.isInvalid)
                {
                    this.isInvalid = false;

                    this.tokenBuilder.Reset();

                    ch = this.SkipToNextRule(ch, ref charClass, ref parseCurrent);
                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }
                }
            }
            while (start < parseCurrent);

            return ch;
        }

        
        private char ScanCdo(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            InternalDebug.Assert(ch == '<');
            parseCurrent++;

            if (parseCurrent + 3 >= this.parseEnd)
            {
                parseCurrent = this.parseEnd;
                return ch;
            }

            if (this.parseBuffer[parseCurrent++] != '!' ||
                this.parseBuffer[parseCurrent++] != '-' ||
                this.parseBuffer[parseCurrent++] != '-')
            {
                return this.SkipToNextRule(ch, ref charClass, ref parseCurrent);
            }

            ch = parseBuffer[parseCurrent];
            charClass = ParseSupport.GetCharClass(ch);
            return ch;
        }

        
        private char ScanCdc(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            InternalDebug.Assert(ch == '-');
            parseCurrent++;

            if (parseCurrent + 2 >= this.parseEnd)
            {
                parseCurrent = this.parseEnd;
                return ch;
            }

            if (this.parseBuffer[parseCurrent++] != '-' ||
                this.parseBuffer[parseCurrent++] != '>')
            {
                return this.SkipToNextRule(ch, ref charClass, ref parseCurrent);
            }

            ch = parseBuffer[parseCurrent];
            charClass = ParseSupport.GetCharClass(ch);
            return ch;
        }

        
        private char ScanAtRule(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            int parseEnd = this.parseEnd;
            char[] parseBuffer = this.parseBuffer;

            int selectorNameRunStart = parseCurrent;

            InternalDebug.Assert(ch == '@');

            ch = parseBuffer[++parseCurrent];
            charClass = ParseSupport.GetCharClass(ch);

            if (!this.IsNameStartCharacter(ch, charClass, parseCurrent))
            {
                this.isInvalid = true;
                return ch;
            }

            this.tokenBuilder.StartRuleSet(selectorNameRunStart, CssTokenId.AtRule);

            if (!this.tokenBuilder.CanAddSelector())
            {
                
                parseCurrent = parseEnd;
                return ch;
            }

            this.tokenBuilder.StartSelectorName();

            int nameLength;

            this.PrepareAndAddRun(CssRunKind.AtRuleName, selectorNameRunStart, ref parseCurrent);
            if (parseCurrent == parseEnd)
            {
                return ch;
            }

            ch = this.ScanName(CssRunKind.AtRuleName, ch, ref charClass, ref parseCurrent, out nameLength);

            InternalDebug.Assert(parseCurrent > selectorNameRunStart);

            this.tokenBuilder.EndSelectorName(nameLength);

            if (parseCurrent == parseEnd)
            {
                return ch;
            }

            if (this.IsNameEqual("page", selectorNameRunStart + 1, parseCurrent - selectorNameRunStart - 1))
            {
                ch = this.ScanPageSelector(ch, ref charClass, ref parseCurrent);
                if (parseCurrent == parseEnd)
                {
                    return ch;
                }
            }
            else if (!this.IsNameEqual("font-face", selectorNameRunStart + 1, parseCurrent - selectorNameRunStart - 1))
            {
                this.isInvalid = true;
                return ch;
            }

            this.tokenBuilder.EndSimpleSelector();

            ch = this.ScanDeclarationBlock(ch, ref charClass, ref parseCurrent);
            if (parseCurrent == parseEnd)
            {
                return ch;
            }

            return ch;
        }

        
        private char ScanPageSelector(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, false);
            if (parseCurrent == parseEnd)
            {
                return ch;
            }

            if (this.IsNameStartCharacter(ch, charClass, parseCurrent))
            {
                this.tokenBuilder.EndSimpleSelector();

                this.tokenBuilder.StartSelectorName();

                int nameLength;

                ch = this.ScanName(CssRunKind.PageIdent, ch, ref charClass, ref parseCurrent, out nameLength);

                this.tokenBuilder.EndSelectorName(nameLength);

                if (parseCurrent == parseEnd)
                {
                    return ch;
                }

                this.tokenBuilder.SetSelectorCombinator(CssSelectorCombinator.Descendant, false);
            }

            if (ch == ':')
            {
                ch = parseBuffer[++parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);

                this.PrepareAndAddRun(CssRunKind.PagePseudoStart, parseCurrent - 1, ref parseCurrent);
                if (parseCurrent == parseEnd)
                {
                    return ch;
                }

                if (!this.IsNameStartCharacter(ch, charClass, parseCurrent))
                {
                    this.tokenBuilder.InvalidateLastValidRun(CssRunKind.SelectorPseudoStart);
                    return ch;
                }

                this.tokenBuilder.StartSelectorClass(CssSelectorClassType.Pseudo);

                int nameLength;

                ch = this.ScanName(CssRunKind.PagePseudo, ch, ref charClass, ref parseCurrent, out nameLength);

                this.tokenBuilder.EndSelectorClass();

                if (parseCurrent == parseEnd)
                {
                    return ch;
                }
            }

            ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, false);
            if (parseCurrent == parseEnd)
            {
                return ch;
            }

            return ch;
        }

        
        private char ScanRuleSet(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            this.tokenBuilder.StartRuleSet(parseCurrent, CssTokenId.RuleSet);

            
            ch = this.ScanSelectors(ch, ref charClass, ref parseCurrent);
            if (parseCurrent == parseEnd || this.isInvalid)
            {
                return ch;
            }

            ch = this.ScanDeclarationBlock(ch, ref charClass, ref parseCurrent);
            if (parseCurrent == parseEnd)
            {
                return ch;
            }

            return ch;
        }

        
        private char ScanDeclarationBlock(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, false);
            if (parseCurrent == parseEnd)
            {
                return ch;
            }

            if (ch != '{')
            {
                this.isInvalid = true;
                return ch;
            }

            this.ruleDepth++;

            ch = parseBuffer[++parseCurrent];
            charClass = ParseSupport.GetCharClass(ch);

            this.PrepareAndAddRun(CssRunKind.Delimiter, parseCurrent - 1, ref parseCurrent);
            if (parseCurrent == parseEnd)
            {
                return ch;
            }

            ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
            if (parseCurrent == parseEnd)
            {
                return ch;
            }

            ch = this.ScanDeclarations(ch, ref charClass, ref parseCurrent);
            if (parseCurrent == parseEnd)
            {
                return ch;
            }

            if (ch != '}')
            {
                this.isInvalid = true;
                return ch;
            }

            
            InternalDebug.Assert(this.ruleDepth > 0);
            this.ruleDepth--;

            ch = parseBuffer[++parseCurrent];
            charClass = ParseSupport.GetCharClass(ch);

            this.PrepareAndAddRun(CssRunKind.Delimiter, parseCurrent - 1, ref parseCurrent);
            if (parseCurrent == parseEnd)
            {
                return ch;
            }

            return ch;
        }

        
        private char ScanSelectors(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            int parseEnd = this.parseEnd;
            char[] parseBuffer = this.parseBuffer;

            int start = parseCurrent;
            ch = this.ScanSimpleSelector(ch, ref charClass, ref parseCurrent);
            if (parseCurrent == parseEnd || this.isInvalid)
            {
                return ch;
            }

            while (start < parseCurrent)
            {
                CssSelectorCombinator combinator = CssSelectorCombinator.None;

                bool cleanupSpace = false;
                bool cleanupCombinator = false;

                start = parseCurrent;
                ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, false);
                if (parseCurrent == parseEnd)
                {
                    return ch;
                }
                if (start < parseCurrent)
                {
                    
                    cleanupSpace = true;

                    combinator = CssSelectorCombinator.Descendant;
                }

                
                if (ch == '+' || ch == '>' || ch == ',')
                {
                    combinator = (ch == '+') ? CssSelectorCombinator.Adjacent :
                        ((ch == '>') ? CssSelectorCombinator.Child : CssSelectorCombinator.None);

                    ch = parseBuffer[++parseCurrent];
                    charClass = ParseSupport.GetCharClass(ch);
                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }

                    
                    this.PrepareAndAddRun(CssRunKind.SelectorCombinatorOrComma, parseCurrent - 1, ref parseCurrent);
                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }

                    cleanupCombinator = true;

                    ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }
                }
                else if (start == parseCurrent)
                {
                    break;
                }

                start = parseCurrent;
                ch = this.ScanSimpleSelector(ch, ref charClass, ref parseCurrent);
                if (start == parseCurrent)
                {
                    if (cleanupCombinator)
                    {
                        this.tokenBuilder.InvalidateLastValidRun(CssRunKind.SelectorCombinatorOrComma);
                    }
                    if (cleanupSpace)
                    {
                        this.tokenBuilder.InvalidateLastValidRun(CssRunKind.Space);
                    }
                    break;
                }
                else
                {
                    if (this.isInvalid)
                    {
                        return ch;
                    }
                    this.tokenBuilder.SetSelectorCombinator(combinator, true);
                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }
                }
            }

            return ch;
        }

        
        private char ScanSimpleSelector(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            int parseEnd = this.parseEnd;
            char[] parseBuffer = this.parseBuffer;

            if (ch == '.' || ch == ':' || ch == '#' || ch == '[')
            {
                if (!this.tokenBuilder.CanAddSelector())
                {
                    
                    parseCurrent = parseEnd;
                    return ch;
                }

                this.tokenBuilder.BuildUniversalSelector();
            }
            else if (this.IsNameStartCharacter(ch, charClass, parseCurrent) || ch == '*')
            {
                if (!this.tokenBuilder.CanAddSelector())
                {
                    
                    parseCurrent = parseEnd;
                    return ch;
                }

                this.tokenBuilder.StartSelectorName();

                int nameLength;

                if (ch == '*')
                {
                    nameLength = 1;

                    ch = parseBuffer[++parseCurrent];
                    charClass = ParseSupport.GetCharClass(ch);

                    this.PrepareAndAddRun(CssRunKind.SelectorName, parseCurrent - 1, ref parseCurrent);
                }
                else
                {
                    ch = this.ScanName(CssRunKind.SelectorName, ch, ref charClass, ref parseCurrent, out nameLength);
                }

                this.tokenBuilder.EndSelectorName(nameLength);
                if (parseCurrent == parseEnd)
                {
                    return ch;
                }
            }
            else
            {
                return ch;
            }

            ch = this.ScanSelectorSuffix(ch, ref charClass, ref parseCurrent);
            this.tokenBuilder.EndSimpleSelector();

            return ch;
        }

        
        private char ScanSelectorSuffix(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            if (ch == '[')
            {
                this.tokenBuilder.StartSelectorClass(CssSelectorClassType.Attrib);
                ch = this.ScanSelectorAttrib(ch, ref charClass, ref parseCurrent);
                this.tokenBuilder.EndSelectorClass();

                return ch;
            }

            int parseEnd = this.parseEnd;
            char[] parseBuffer = this.parseBuffer;

            if (ch == ':')
            {
                ch = parseBuffer[++parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);

                this.PrepareAndAddRun(CssRunKind.SelectorPseudoStart, parseCurrent - 1, ref parseCurrent);
                if (parseCurrent == parseEnd)
                {
                    return ch;
                }

                this.tokenBuilder.StartSelectorClass(CssSelectorClassType.Pseudo);
                ch = this.ScanSelectorPseudo(ch, ref charClass, ref parseCurrent);
                this.tokenBuilder.EndSelectorClass();

                return ch;
            }

            if (ch == '.' || ch == '#')
            {
                bool isClass = ch == '.';

                ch = parseBuffer[++parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);

                if (this.IsNameCharacter(ch, charClass, parseCurrent) && (!isClass || this.IsNameStartCharacter(ch, charClass, parseCurrent)))
                {
                    this.PrepareAndAddRun(isClass ? CssRunKind.SelectorClassStart : CssRunKind.SelectorHashStart, parseCurrent - 1, ref parseCurrent);

                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }

                    this.tokenBuilder.StartSelectorClass(isClass ? CssSelectorClassType.Regular : CssSelectorClassType.Hash);

                    int nameLength;

                    ch = this.ScanName(isClass ? CssRunKind.SelectorClass : CssRunKind.SelectorHash, ch, ref charClass, ref parseCurrent, out nameLength);

                    this.tokenBuilder.EndSelectorClass();

                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }
                }
                else
                {
                    this.PrepareAndAddInvalidRun(CssRunKind.FunctionStart, ref parseCurrent);
                }
            }

            return ch;
        }

        
        private char ScanSelectorPseudo(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            int parseEnd = this.parseEnd;
            char[] parseBuffer = this.parseBuffer;

            if (!this.IsNameStartCharacter(ch, charClass, parseCurrent))
            {
                this.tokenBuilder.InvalidateLastValidRun(CssRunKind.SelectorPseudoStart);
                return ch;
            }

            int constructRunStart = parseCurrent;

            int nameLength;

            ch = this.ScanName(CssRunKind.SelectorPseudo, ch, ref charClass, ref parseCurrent, out nameLength);

            if (parseCurrent == parseEnd)
            {
                return ch;
            }

            if (ch == '(')
            {
                if (!this.IsSafeIdentifier(SafePseudoFunctions, constructRunStart, parseCurrent))
                {
                    return ch;
                }

                ch = parseBuffer[++parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);
                if (parseCurrent == parseEnd)
                {
                    
                    return ch;
                }

                
                this.PrepareAndAddRun(CssRunKind.FunctionStart, parseCurrent - 1, ref parseCurrent);
                if (parseCurrent == parseEnd)
                {
                    
                    return ch;
                }

                ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
                if (parseCurrent == parseEnd)
                {
                    
                    return ch;
                }

                if (!this.IsNameStartCharacter(ch, charClass, parseCurrent))
                {
                    

                    this.isInvalid = true;
                    return ch;
                }

                ch = this.ScanName(CssRunKind.SelectorPseudoArg, ch, ref charClass, ref parseCurrent, out nameLength);

                if (parseCurrent == parseEnd)
                {
                    
                    return ch;
                }

                ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
                if (parseCurrent == parseEnd)
                {
                    
                    return ch;
                }

                if (ch != ')')
                {
                    this.isInvalid = true;
                    return ch;
                }

                ch = parseBuffer[++parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);

                
                this.PrepareAndAddRun(CssRunKind.FunctionEnd, parseCurrent - 1, ref parseCurrent);
                if (parseCurrent == parseEnd)
                {
                    return ch;
                }
            }

            return ch;
        }

        
        private char ScanSelectorAttrib(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            int parseEnd = this.parseEnd;
            char[] parseBuffer = this.parseBuffer;

            InternalDebug.Assert(ch == '[');

            ch = parseBuffer[++parseCurrent];
            charClass = ParseSupport.GetCharClass(ch);

            this.PrepareAndAddRun(CssRunKind.SelectorAttribStart, parseCurrent - 1, ref parseCurrent);
            if (parseCurrent == parseEnd)
            {
                return ch;
            }

            ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
            if (parseCurrent == parseEnd)
            {
                return ch;
            }

            if (!this.IsNameStartCharacter(ch, charClass, parseCurrent))
            {
                this.isInvalid = true;
                return ch;
            }

            int nameLength;

            ch = this.ScanName(CssRunKind.SelectorAttribName, ch, ref charClass, ref parseCurrent, out nameLength);

            if (parseCurrent == parseEnd)
            {
                return ch;
            }

            ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
            if (parseCurrent == parseEnd)
            {
                return ch;
            }

            int constructRunStart = parseCurrent;
            if (ch == '=')
            {
                ch = parseBuffer[++parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);

                this.PrepareAndAddRun(CssRunKind.SelectorAttribEquals, parseCurrent - 1, ref parseCurrent);
                if (parseCurrent == parseEnd)
                {
                    return ch;
                }
            }
            else if ((ch == '~' || ch == '|') && parseBuffer[parseCurrent + 1] == '=')
            {
                parseCurrent += 2;

                this.PrepareAndAddRun(
                    ch == '~' ? CssRunKind.SelectorAttribIncludes : CssRunKind.SelectorAttribDashmatch,
                    parseCurrent - 2,
                    ref parseCurrent);

                if (parseCurrent == parseEnd)
                {
                    return ch;
                }

                ch = parseBuffer[parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);
            }

            if (constructRunStart < parseCurrent)
            {
                
                ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
                if (parseCurrent == parseEnd)
                {
                    return ch;
                }

                if (this.IsNameStartCharacter(ch, charClass, parseCurrent))
                {
                    constructRunStart = parseCurrent;
                    ch = this.ScanName(CssRunKind.SelectorAttribIdentifier, ch, ref charClass, ref parseCurrent, out nameLength);

                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }
                }
                else if (ch == '"' || ch == '\'')
                {
                    constructRunStart = parseCurrent;
                    ch = this.ScanString(ch, ref charClass, ref parseCurrent, false);

                    
                    this.PrepareAndAddRun(CssRunKind.SelectorAttribString, constructRunStart, ref parseCurrent);

                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }
                }

                ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
                if (parseCurrent == parseEnd)
                {
                    return ch;
                }
            }

            if (ch != ']')
            {
                this.isInvalid = true;
                return ch;
            }

            ch = parseBuffer[++parseCurrent];
            charClass = ParseSupport.GetCharClass(ch);

            this.PrepareAndAddRun(CssRunKind.SelectorAttribEnd, parseCurrent - 1, ref parseCurrent);
            if (parseCurrent == parseEnd)
            {
                return ch;
            }

            return ch;
        }

        
        private char ScanDeclarations(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            int parseEnd = this.parseEnd;
            char[] parseBuffer = this.parseBuffer;

            this.tokenBuilder.StartDeclarations(parseCurrent);
            int start;

            do
            {
                start = parseCurrent;

                ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
                if (parseCurrent == parseEnd)
                {
                    return ch;
                }

                if (this.IsNameStartCharacter(ch, charClass, parseCurrent))
                {
                    if (!this.tokenBuilder.CanAddProperty())
                    {
                        
                        parseCurrent = parseEnd;
                        return ch;
                    }

                    this.tokenBuilder.StartPropertyName();

                    int nameLength;

                    ch = this.ScanName(CssRunKind.PropertyName, ch, ref charClass, ref parseCurrent, out nameLength);

                    this.tokenBuilder.EndPropertyName(nameLength);

                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }

                    ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }

                    if (ch != ':')
                    {
                        this.tokenBuilder.MarkPropertyAsDeleted();
                        return ch;
                    }

                    ch = parseBuffer[++parseCurrent];
                    charClass = ParseSupport.GetCharClass(ch);

                    this.PrepareAndAddRun(CssRunKind.PropertyColon, parseCurrent - 1, ref parseCurrent);
                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }

                    ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }

                    this.tokenBuilder.StartPropertyValue();

                    ch = this.ScanPropertyValue(ch, ref charClass, ref parseCurrent);
                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }

                    this.tokenBuilder.EndPropertyValue();
                    this.tokenBuilder.EndProperty();

                    ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }
                }

                if (ch != ';')
                {
                    this.tokenBuilder.EndDeclarations();
                    return ch;
                }

                ch = parseBuffer[++parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);

                this.PrepareAndAddRun(CssRunKind.Delimiter, parseCurrent - 1, ref parseCurrent);
                if (parseCurrent == parseEnd)
                {
                    return ch;
                }
            }
            while (start < parseCurrent);

            return ch;
        }

        
        private char ScanPropertyValue(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            int parseEnd = this.parseEnd;
            char[] parseBuffer = this.parseBuffer;

            ch = this.ScanExpr(ch, ref charClass, ref parseCurrent, 0);
            if (parseCurrent == parseEnd)
            {
                return ch;
            }

            if (ch == '!')
            {
                ch = parseBuffer[++parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);
                if (parseCurrent == parseEnd)
                {
                    this.tokenBuilder.MarkPropertyAsDeleted();
                    return ch;
                }

                
                this.PrepareAndAddRun(CssRunKind.ImportantStart, parseCurrent - 1, ref parseCurrent);
                if (parseCurrent == parseEnd)
                {
                    return ch;
                }

                ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
                if (parseCurrent == parseEnd)
                {
                    this.tokenBuilder.MarkPropertyAsDeleted();
                    return ch;
                }

                if (this.IsNameStartCharacter(ch, charClass, parseCurrent))
                {
                    int identStart = parseCurrent;
                    int nameLength;

                    ch = this.ScanName(CssRunKind.Important, ch, ref charClass, ref parseCurrent, out nameLength);

                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }

                    if (!this.IsNameEqual("important", identStart, parseCurrent - identStart))
                    {
                        this.tokenBuilder.MarkPropertyAsDeleted();
                        return ch;
                    }
                }
                else
                {
                    this.tokenBuilder.MarkPropertyAsDeleted();
                    return ch;
                }
            }

            return ch;
        }

        
        private char ScanExpr(char ch, ref CharClass charClass, ref int parseCurrent, int level)
        {
            int parseEnd = this.parseEnd;
            char[] parseBuffer = this.parseBuffer;

            int start = parseCurrent;
            ch = this.ScanTerm(ch, ref charClass, ref parseCurrent, level);
            if (parseCurrent == parseEnd)
            {
                return ch;
            }

            while (start < parseCurrent)
            {
                bool cleanupSpace = false;
                bool cleanupOperator = false;

                start = parseCurrent;
                ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, false);
                if (parseCurrent == parseEnd)
                {
                    return ch;
                }
                if (start < parseCurrent)
                {
                    
                    cleanupSpace = true;
                }

                
                if (ch == '/' || ch == ',')
                {
                    ch = parseBuffer[++parseCurrent];
                    charClass = ParseSupport.GetCharClass(ch);
                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }

                    
                    this.PrepareAndAddRun(CssRunKind.Operator, parseCurrent - 1, ref parseCurrent);
                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }

                    cleanupOperator = true;

                    ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }
                }
                else if (start == parseCurrent)
                {
                    break;
                }

                start = parseCurrent;
                ch = this.ScanTerm(ch, ref charClass, ref parseCurrent, level);
                if (parseCurrent == parseEnd)
                {
                    return ch;
                }

                if (start == parseCurrent)
                {
                    if (cleanupOperator)
                    {
                        this.tokenBuilder.InvalidateLastValidRun(CssRunKind.Operator);
                    }
                    if (cleanupSpace)
                    {
                        this.tokenBuilder.InvalidateLastValidRun(CssRunKind.Space);
                    }
                    break;
                }
            }

            return ch;
        }

        
        private char ScanTerm(char ch, ref CharClass charClass, ref int parseCurrent, int level)
        {
            int parseEnd = this.parseEnd;
            char[] parseBuffer = this.parseBuffer;
            int start, runStart;
            bool unaryOperator = false;

            
            if (ch == '-' || ch == '+')
            {
                ch = parseBuffer[++parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);
                if (parseCurrent == parseEnd)
                {
                    this.tokenBuilder.MarkPropertyAsDeleted();
                    return ch;
                }

                
                this.PrepareAndAddRun(CssRunKind.UnaryOperator, parseCurrent - 1, ref parseCurrent);
                if (parseCurrent == parseEnd)
                {
                    return ch;
                }

                unaryOperator = true;
            }

            if (ParseSupport.NumericCharacter(charClass) || ch == '.')
            {
                ch = this.ScanNumeric(ch, ref charClass, ref parseCurrent);
                if (parseCurrent == parseEnd)
                {
                    return ch;
                }

                if (ch == '.')
                {
                    ch = parseBuffer[++parseCurrent];
                    charClass = ParseSupport.GetCharClass(ch);

                    
                    this.PrepareAndAddRun(CssRunKind.Dot, parseCurrent - 1, ref parseCurrent);

                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }

                    start = parseCurrent;
                    ch = this.ScanNumeric(ch, ref charClass, ref parseCurrent);
                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }

                    if (start == parseCurrent)
                    {
                        this.tokenBuilder.MarkPropertyAsDeleted();
                    }
                }

                if (ch == '%')
                {
                    ch = parseBuffer[++parseCurrent];
                    charClass = ParseSupport.GetCharClass(ch);

                    
                    this.PrepareAndAddRun(CssRunKind.Percent, parseCurrent - 1, ref parseCurrent);

                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }
                }
                else if (this.IsNameStartCharacter(ch, charClass, parseCurrent))
                {
                    

                    int nameLength;

                    ch = this.ScanName(CssRunKind.Metrics, ch, ref charClass, ref parseCurrent, out nameLength);

                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }
                }
            }
            else if (this.IsNameStartCharacter(ch, charClass, parseCurrent))
            {
                

                start = parseCurrent;

                int nameLength;

                ch = this.ScanName(CssRunKind.TermIdentifier, ch, ref charClass, ref parseCurrent, out nameLength);

                if (parseCurrent == parseEnd)
                {
                    return ch;
                }

                runStart = parseCurrent;

                if (ch == '+' && (start + 1) == parseCurrent && (parseBuffer[start] == 'u' || parseBuffer[start] == 'U'))
                {
                    ch = this.ScanUnicodeRange(ch, ref charClass, ref parseCurrent);

                    
                    this.PrepareAndAddRun(CssRunKind.UnicodeRange, runStart, ref parseCurrent);

                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }
                }
                else if (ch == '(')
                {
                    bool isUrl = false;

                    if (!this.IsSafeIdentifier(SafeTermFunctions, start, parseCurrent))
                    {
                        this.tokenBuilder.MarkPropertyAsDeleted();

                        if (this.IsNameEqual("url", start, parseCurrent - start))
                        {
                            isUrl = true;
                        }
                    }

                    ch = parseBuffer[++parseCurrent];
                    charClass = ParseSupport.GetCharClass(ch);
                    if (parseCurrent == parseEnd)
                    {
                        
                        this.tokenBuilder.MarkPropertyAsDeleted();
                        return ch;
                    }

                    
                    this.PrepareAndAddRun(CssRunKind.FunctionStart, parseCurrent - 1, ref parseCurrent);
                    if (parseCurrent == parseEnd)
                    {
                        
                        this.tokenBuilder.MarkPropertyAsDeleted();
                        return ch;
                    }

                    ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
                    if (parseCurrent == parseEnd)
                    {
                        
                        this.tokenBuilder.MarkPropertyAsDeleted();
                        return ch;
                    }

                    if (isUrl)
                    {
                        if (ch == '"' || ch == '\'')
                        {
                            start = parseCurrent;
                            ch = this.ScanString(ch, ref charClass, ref parseCurrent, true);

                            
                            this.PrepareAndAddRun(CssRunKind.String, start, ref parseCurrent);

                            if (parseCurrent == parseEnd)
                            {
                                return ch;
                            }
                        }
                        else
                        {
                            

                            start = parseCurrent;
                            ch = this.ScanUrl(ch, ref charClass, ref parseCurrent);

                            if (parseCurrent == parseEnd)
                            {
                                return ch;
                            }
                        }

                        ch = this.ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
                        if (parseCurrent == parseEnd)
                        {
                            return ch;
                        }
                    }
                    else
                    {
                        if (++level > 16)
                        {
                            
                            this.tokenBuilder.MarkPropertyAsDeleted();
                            return ch;
                        }
                        ch = this.ScanExpr(ch, ref charClass, ref parseCurrent, level);
                        if (parseCurrent == parseEnd)
                        {
                            
                            this.tokenBuilder.MarkPropertyAsDeleted();
                            return ch;
                        }
                    }

                    if (ch != ')')
                    {
                        this.tokenBuilder.MarkPropertyAsDeleted();
                    }

                    ch = parseBuffer[++parseCurrent];
                    charClass = ParseSupport.GetCharClass(ch);

                    
                    this.PrepareAndAddRun(CssRunKind.FunctionEnd, parseCurrent - 1, ref parseCurrent);
                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }
                }
                else if (unaryOperator)
                {
                    this.tokenBuilder.MarkPropertyAsDeleted();
                }
            }
            else if (unaryOperator)
            {
                this.tokenBuilder.MarkPropertyAsDeleted();
            }
            else if (ch == '#')
            {
                ch = parseBuffer[++parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);

                
                this.PrepareAndAddRun(CssRunKind.HexColorStart, parseCurrent - 1, ref parseCurrent);

                if (parseCurrent == parseEnd)
                {
                    return ch;
                }

                if (this.IsNameCharacter(ch, charClass, parseCurrent))
                {
                    

                    int nameLength;

                    ch = this.ScanName(CssRunKind.HexColor, ch, ref charClass, ref parseCurrent, out nameLength);

                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }
                }
                else
                {
                    this.tokenBuilder.MarkPropertyAsDeleted();
                }
            }
            else if (ch == '"' || ch == '\'')
            {
                start = parseCurrent;
                ch = this.ScanString(ch, ref charClass, ref parseCurrent, true);

                
                this.PrepareAndAddRun(CssRunKind.String, start, ref parseCurrent);

                if (parseCurrent == parseEnd)
                {
                    return ch;
                }
            }

            return ch;
        }

        
        private char ScanNumeric(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            int start = parseCurrent;
            char[] parseBuffer = this.parseBuffer;

            while (ParseSupport.NumericCharacter(charClass))
            {
                ch = parseBuffer[++parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);
            }

            
            this.PrepareAndAddRun(CssRunKind.Numeric, start, ref parseCurrent);

            return ch;
        }

        
        private char ScanString(char ch, ref CharClass charClass, ref int parseCurrent, bool inProperty)
        {
            int parseEnd = this.parseEnd;
            char[] parseBuffer = this.parseBuffer;

            
            
            char term = ch;
            
            
            char chLast = '\0';
            char ch2ndLast = '\0';

            while (true)
            {
                ch = parseBuffer[++parseCurrent];

                if (parseCurrent == parseEnd)
                {
                    
                    
                    if (inProperty)
                    {
                        this.tokenBuilder.MarkPropertyAsDeleted();
                    }
                    charClass = ParseSupport.GetCharClass(ch);
                    return ch;
                }

                
                
                if (CssToken.AttemptUnescape(parseBuffer, parseEnd, ref ch, ref parseCurrent))
                {
                    if (parseCurrent == parseEnd)
                    {
                        
                        
                        if (inProperty)
                        {
                            this.tokenBuilder.MarkPropertyAsDeleted();
                        }
                        charClass = ParseSupport.GetCharClass(parseBuffer[parseCurrent]);
                        return parseBuffer[parseCurrent];
                    }
                    
                    
                    chLast = '\0';
                    ch2ndLast = '\0';
                }
                else
                {
                    
                    
                    if (ch == term || 
                        (ch == '\n' && chLast == '\r' && ch2ndLast != '\\') || 
                        (((ch == '\n' && chLast != '\r') || ch == '\r' || ch == '\f') && chLast != '\\')) 
                    {
                        break;
                    }
                    ch2ndLast = chLast;
                    chLast = ch;
                }
            }

            ch = parseBuffer[++parseCurrent];
            charClass = ParseSupport.GetCharClass(ch);

            return ch;
        }

        
        private char ScanName(CssRunKind runKind, char ch, ref CharClass charClass, ref int parseCurrent, out int nameLength)
        {
            nameLength = 0;

            while (true)
            {
                int runStart = parseCurrent;

                while (IsNameCharacterNoEscape(ch, ParseSupport.GetCharClass(ch)))
                {
                    nameLength++;
                    if (parseCurrent == this.parseEnd)
                    {
                        break;
                    }
                    ch = parseBuffer[++parseCurrent];
                }

                if (parseCurrent != runStart)
                {
                    this.PrepareAndAddRun(runKind, runStart, ref parseCurrent);
                }

                if (parseCurrent != this.parseEnd)
                {
                    runStart = parseCurrent;

                    if (ch != '\\')
                    {
                        break;
                    }

                    if (!CssToken.AttemptUnescape(this.parseBuffer, this.parseEnd, ref ch, ref parseCurrent))
                    {
                        
                        
                        ch = parseBuffer[++parseCurrent];
                        this.PrepareAndAddInvalidRun(runKind, ref parseCurrent);
                        break;
                    }

                    
                    
                    
                    
                    
                    
                    ++parseCurrent;
                    if (!IsNameCharacterNoEscape(ch, ParseSupport.GetCharClass(ch)))
                    {
                        if (parseCurrent != runStart)
                        {
                            this.PrepareAndAddLiteralRun(runKind, runStart, ref parseCurrent, ch);
                        }
                        nameLength = 0;
                        break;
                    }
                    

                    nameLength++;

                    this.PrepareAndAddLiteralRun(runKind, runStart, ref parseCurrent, ch);

                    if (parseCurrent == this.parseEnd)
                    {
                        break;
                    }
                    ch = parseBuffer[parseCurrent];
                }
                else
                {
                    break;
                }
            }

            charClass = ParseSupport.GetCharClass(ch);

            return ch;
        }

        
        private char ScanUrl(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            while (true)
            {
                int runStart = parseCurrent;

                while (IsUrlCharacter(ch, ParseSupport.GetCharClass(ch), parseCurrent) && parseCurrent != this.parseEnd)
                {
                    ch = parseBuffer[++parseCurrent];
                }

                if (parseCurrent != runStart)
                {
                    this.PrepareAndAddRun(CssRunKind.Url, runStart, ref parseCurrent);
                }

                if (parseCurrent != this.parseEnd)
                {
                    runStart = parseCurrent;

                    if (ch != '\\')
                    {
                        break;
                    }

                    if (!CssToken.AttemptUnescape(this.parseBuffer, this.parseEnd, ref ch, ref parseCurrent))
                    {
                        ch = parseBuffer[++parseCurrent];
                        this.PrepareAndAddInvalidRun(CssRunKind.Url, ref parseCurrent);
                        break;
                    }

                    ++parseCurrent;
                    this.PrepareAndAddLiteralRun(CssRunKind.Url, runStart, ref parseCurrent, ch);

                    if (parseCurrent == this.parseEnd)
                    {
                        break;
                    }
                    ch = parseBuffer[parseCurrent];
                }
                else
                {
                    break;
                }
            }

            charClass = ParseSupport.GetCharClass(ch);

            return ch;
        }

        
        private char ScanUnicodeRange(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            InternalDebug.Assert(ch == '+');

            char[] parseBuffer = this.parseBuffer;

            char chT = ch;
            int specStart = parseCurrent + 1;
            int i = specStart;
            bool noQuestion = true;

            for (; i < specStart + 6; i++)
            {
                chT = parseBuffer[i];
                if ('?' == chT)
                {
                    noQuestion = false;
                    i++;
                    for (; i < specStart + 6 && '?' == parseBuffer[i]; i++)
                    {
                    }
                    break;
                }
                else if (!ParseSupport.HexCharacter(ParseSupport.GetCharClass(chT)))
                {
                    if (i == specStart)
                    {
                        
                        return ch;
                    }
                    break;
                }
            }

            chT = parseBuffer[i];
            if ('-' == chT && noQuestion)
            {
                i++;
                specStart = i;
                for (; i < specStart + 6; i++)
                {
                    chT = parseBuffer[i];
                    if (!ParseSupport.HexCharacter(ParseSupport.GetCharClass(chT)))
                    {
                        if (i == specStart)
                        {
                            
                            return ch;
                        }
                        break;
                    }
                }
            }

            
            chT = parseBuffer[i];
            charClass = ParseSupport.GetCharClass(chT);
            parseCurrent = i;
            return chT;
        }

        
        private char ScanWhitespace(char ch, ref CharClass charClass, ref int parseCurrent, bool ignorable)
        {
            char[] parseBuffer = this.parseBuffer;
            int parseEnd = this.parseEnd;

            while (ParseSupport.WhitespaceCharacter(charClass) || ch == '/')
            {
                if (ch == '/')
                {
                    if (parseCurrent < parseEnd && parseBuffer[parseCurrent + 1] == '*')
                    {
                        ch = this.ScanComment(ch, ref charClass, ref parseCurrent);
                        if (parseCurrent == parseEnd)
                        {
                            return ch;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    int runStart = parseCurrent;

                    do
                    {
                        if (++parseCurrent == parseEnd)
                        {
                            return ch;
                        }
                        ch = parseBuffer[parseCurrent];
                        charClass = ParseSupport.GetCharClass(ch);
                    }
                    while (ParseSupport.WhitespaceCharacter(charClass));

                    if (this.tokenBuilder.IsStarted)
                    {
                        
                        this.PrepareAndAddRun(ignorable ? CssRunKind.Invalid : CssRunKind.Space, runStart, ref parseCurrent);
                    }
                }
            }

            return ch;
        }

        
        private char ScanComment(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            char[] parseBuffer = this.parseBuffer;
            int parseEnd = this.parseEnd;

            int runStart = parseCurrent;

            InternalDebug.Assert(ch == '/');
            ch = parseBuffer[++parseCurrent];

            InternalDebug.Assert(ch == '*');

            while (true)
            {
                do
                {
                    if (++parseCurrent == parseEnd)
                    {
                        return ch;
                    }
                }
                while (parseBuffer[parseCurrent] != '*');

                if (parseCurrent + 1 != parseEnd && parseBuffer[parseCurrent + 1] == '/')
                {
                    parseCurrent++;
                    if (++parseCurrent == parseEnd)
                    {
                        return ch;
                    }

                    if (this.tokenBuilder.IsStarted)
                    {
                        
                        this.PrepareAndAddRun(CssRunKind.Comment, runStart, ref parseCurrent);
                    }

                    ch = parseBuffer[parseCurrent];
                    charClass = ParseSupport.GetCharClass(ch);
                    return ch;
                }
            }
        }

        
        private void PrepareAndAddRun(CssRunKind runKind, int start, ref int parseCurrent)
        {
            if (!this.tokenBuilder.PrepareAndAddRun(runKind, start, parseCurrent))
            {
                parseCurrent = this.parseEnd;
            }
        }

        
        private void PrepareAndAddInvalidRun(CssRunKind runKind, ref int parseCurrent)
        {
            if (!this.tokenBuilder.PrepareAndAddInvalidRun(runKind, parseCurrent))
            {
                parseCurrent = this.parseEnd;
            }
        }

        
        private void PrepareAndAddLiteralRun(CssRunKind runKind, int start, ref int parseCurrent, int value)
        {
            if (!this.tokenBuilder.PrepareAndAddLiteralRun(runKind, start, parseCurrent, value))
            {
                parseCurrent = this.parseEnd;
            }
        }

        
        private char SkipToNextRule(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            int parseEnd = this.parseEnd;
            char[] parseBuffer = this.parseBuffer;

            while (true)
            {
                if (ch == '"' || ch == '\'')
                {
                    ch = this.ScanString(ch, ref charClass, ref parseCurrent, false);
                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }
                    continue;
                }
                else if (ch == '{')
                {
                    this.ruleDepth++;
                }
                else if (ch == '}')
                {
                    if (this.ruleDepth > 0)
                    {
                        this.ruleDepth--;
                    }

                    if (this.ruleDepth == 0)
                    {
                        ch = parseBuffer[++parseCurrent];
                        charClass = ParseSupport.GetCharClass(ch);

                        return ch;
                    }
                }
                else if (ch == ';' && this.ruleDepth == 0)
                {
                    ch = parseBuffer[++parseCurrent];
                    charClass = ParseSupport.GetCharClass(ch);

                    return ch;
                }

                if (++parseCurrent == parseEnd)
                {
                    return ch;
                }
                ch = parseBuffer[parseCurrent];
            }
        }

        
        private bool IsSafeIdentifier(string[] table, int start, int end)
        {
            char[] parseBuffer = this.parseBuffer;
            int length = end - start;

            for (int i = 0; i < table.Length; i++)
            {
                if (this.IsNameEqual(table[i], start, length))
                {
                    return true;
                }
            }

            return false;
        }

        
        private bool IsNameEqual(string name, int start, int length)
        {
            return name.Equals(new string(this.parseBuffer, start, length), StringComparison.OrdinalIgnoreCase);
        }

        
        private bool IsNameCharacter(char ch, CharClass charClass, int parseCurrent)
        {
            return (this.IsNameStartCharacter(ch, charClass, parseCurrent) || ParseSupport.NumericCharacter(charClass) || ch == '-');
        }

        
        private bool IsNameStartCharacter(char ch, CharClass charClass, int parseCurrent)
        {
            if (IsNameStartCharacterNoEscape(ch, charClass))
                return true;
            
            
            if (CssToken.AttemptUnescape(this.parseBuffer, this.parseEnd, ref ch, ref parseCurrent))
            {
                charClass = ParseSupport.GetCharClass(ch);
                return IsNameStartCharacterNoEscape(ch, charClass);
            }
            return false;
        }

        
        private static bool IsNameCharacterNoEscape(char ch, CharClass charClass)
        {
            return (IsNameStartCharacterNoEscape(ch, charClass) || ParseSupport.NumericCharacter(charClass) || ch == '-');
        }

        
        private static bool IsNameStartCharacterNoEscape(char ch, CharClass charClass)
        {
            return (ParseSupport.AlphaCharacter(charClass) || ch == '_' || ch > 127);
        }

        
        private bool IsUrlCharacter(char ch, CharClass charClass, int parseCurrent)
        {
            
            
            
            
            
            return (IsUrlCharacterNoEscape(ch, charClass) || this.IsEscape(ch, parseCurrent));
        }

        
        private static bool IsUrlCharacterNoEscape(char ch, CharClass charClass)
        {
            return (ch >= '*' && ch != 127) || (ch >= '#' && ch <= '&') || ch == '!';
        }

        
        private bool IsEscape(char ch, int parseCurrent)
        {
            return CssToken.AttemptUnescape(this.parseBuffer, this.parseEnd, ref ch, ref parseCurrent);
        }
    }
}

