using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Gerber
{
    public class GerberFileProcessor : StreamReader
    {
        public GerberFileProcessor(string filePath) : base(filePath)
        {
            GraphicState = new GraphicsState();
        }

        //appertures dictionary (includes AM)

        GraphicsState GraphicState { get; set; }

        //ImageItems

        public void Process()
        {
            int read = -1;

            while ((read = Read()) >= 0)
            {
                var chr = (char)read;

                switch (chr)
                {
                    case 'G':
                        ParseGCode();
                        break;
                    case 'D':
                        ParseDCode();
                        break;
                    case 'M':
                        ParseMCode();
                        break;
                    case 'X':
                        var coordX = ParseIntCoord(Axes.X);
                        if (GraphicState.Format.Coordinate == GerberCoordinate.Incremental)
                            GraphicState.CurrentX += coordX;
                        else GraphicState.CurrentX = coordX;
                        break;
                    case 'Y':
                        var coordY = ParseIntCoord(Axes.Y);
                        if (GraphicState.Format.Coordinate == GerberCoordinate.Incremental)
                            GraphicState.CurrentY += coordY;
                        else GraphicState.CurrentY = coordY;
                        break;
                    case 'I':
                        var coordI = ParseIntCoord(Axes.X);
                        GraphicState.OffsetX = coordI;
                        break;
                    case 'J':
                        var coordJ = ParseIntCoord(Axes.Y);
                        GraphicState.OffsetY = coordJ;
                        break;
                    case '%':
                        while (true)//?!?
                        {
                            ParseGerber274X();
                            var c = Read();
                            //skip white spaces
                            while (char.IsWhiteSpace((char)c))
                                c = Read();
                            if (EndOfStream || (char)c == '%')
                                break;

                            //they do this...:
                            //// loop again to catch multiple blocks on the same line (separated by * char)
                            //fd->ptr--;
                        }
                        break;
                    case '*':
                        //this would be the place where we would create a draw object
                        ///* don't even bother saving the net if the aperture state is GERBV_APERTURE_STATE_OFF and we
                        //aren't starting a polygon fill (where we need it to get to the start point) */
                        //                if ((state->aperture_state == GERBV_APERTURE_STATE_OFF) && (!state->in_parea_fill) &&
                        //                        (state->interpolation != GERBV_INTERPOLATION_PAREA_START))
                        //                {
                        //                    /* save the coordinate so the next net can use it for a start point */
                        //                    state->prev_x = state->curr_x;
                        //                    state->prev_y = state->curr_y;
                        //                    break;
                        //                }
                        // create geometry
                        // ...
                        // state->prev_x = state->curr_x;
                        // state->prev_y = state->curr_y;
                        break;

                }
            }
        }

        void ParseMCode()
        {
            throw new NotImplementedException();
            ////if (MCode =={ 1, 2, 3}) //foundEOF
            //var code = ParseInt();
            //switch (code)
            //{
            //    case 2://end of program
            //        break;
            //}
        }

        void ParseDCode()
        {
            var code = ParseInt();
            switch (code)
            {
                case 1: //exposure on
                    GraphicState.ApertureState = ApertureState.On;
                    break;
                case 2://exposure off
                    GraphicState.ApertureState = ApertureState.Off;
                    break;
                case 3://flash aperture
                    GraphicState.ApertureState = ApertureState.Flash;
                    break;

                default://aperture number to use
                    if (code >= 10)//only aperture codes >= 10 are valid
                        GraphicState.CurrentAperture = code;
                    break;
            }
        }

        void ParseGCode()
        {
            var code = ParseInt();
            switch (code)
            {
                case 1: //linear interpolation
                    GraphicState.Interpolation = InterpolationMode.Linear;
                    break;
                case 2: //circular clockwise interpolation
                    GraphicState.Interpolation = InterpolationMode.CircularClockwise;
                    break; //circular CCW interpolation
                case 3:
                    GraphicState.Interpolation = InterpolationMode.CircularCounterClockwise;
                    break;
                case 4: //comment
                    //just read
                    var c = Read();
                    while (!EndOfStream && (char)c != '*')
                        c = Read();
                    break;//case
                case 36: //start region mode
                    GraphicState.PreviousInterpolation = GraphicState.Interpolation;
                    GraphicState.Interpolation = InterpolationMode.RegionModeStart;
                    break;
                case 37: //end region mode
                    GraphicState.Interpolation = InterpolationMode.RegionModeEnd;
                    break;
                case 74: //single quadrant mode
                    GraphicState.QuadrantMode = QuadrantMode.Single;
                    break;
                case 75: //multi quadrant mode
                    GraphicState.QuadrantMode = QuadrantMode.Multi;
                    break;

            }
        }

        int ParseIntCoord(Axes axes)
        {
            //pad the int string according to Leading or Trailing settting
            var value = ParseInt();
            var formatLength = axes == Axes.X ? GraphicState.Format.XInt + GraphicState.Format.XDec :
                                             GraphicState.Format.YInt + GraphicState.Format.YDec;
            var coordStr = GraphicState.Format.OmmitZeroes == OmitZeros.Leading ? value.ToString().PadLeft(formatLength, '0') :
                                                                                  value.ToString().PadRight(formatLength, '0');
            
            int.TryParse(coordStr, out int intCoord);

            return intCoord;
        }

        int ParseInt()
        {
            return 0;
        }

        object ParseApertureDefinition()
        {
            if ((char)Read() == 'D')
            {
                var aperture = new ApertureDefinition();
                var apNo = ParseInt();
                var apStr = GetString('*');
                var tokens = apStr.Split(',');
                switch(tokens[0])
                {
                    case ApertureTypes.Circle:
                        aperture.ApertureType = tokens[0];
                        break;
                    case ApertureTypes.Rectangle:
                        aperture.ApertureType = tokens[0];
                        break;
                    case ApertureTypes.Obround:
                        aperture.ApertureType = tokens[0];
                        break;
                    case ApertureTypes.Polygon:
                        aperture.ApertureType = tokens[0];
                        break;
                }
                if(tokens.Length>=1)
                {
                    var parameters = tokens[1].Split('X');
                    aperture.Parameters = parameters.Select(p => double.Parse(p)).ToArray();
                }
            }

            return null;
        }

        void ParseGerber274X()
        {
            var strCode = ((char)Read()).ToString();
            if (EndOfStream)
                return;
            strCode += (char)Read();
            if (EndOfStream)
                return;
            switch (strCode)
            {
                case "FS"://Format statement
                    {
                        //create new format
                        GraphicState.Format = new FormatStatement();
                        //Leading or Trailing
                        switch ((char)Read())
                        {
                            case 'L':
                                GraphicState.Format.OmmitZeroes = OmitZeros.Leading;
                                break;
                            case 'T':
                                GraphicState.Format.OmmitZeroes = OmitZeros.Trailing;
                                break;
                        }

                        //Absolute or Incremental
                        switch ((char)Read())
                        {
                            case 'A':
                                GraphicState.Format.Coordinate = GerberCoordinate.Absolute;
                                break;
                            case 'I':
                                GraphicState.Format.Coordinate = GerberCoordinate.Incremental;
                                break;
                        }

                        var op = (char)Read();
                        while (!EndOfStream && op != '*')
                        {
                            switch (op)
                            {
                                case 'X':
                                    GraphicState.Format.XInt = op - '0';
                                    op = (char)Read();
                                    GraphicState.Format.XDec = op - '0';
                                    break;
                                case 'Y':
                                    GraphicState.Format.YInt = op - '0';
                                    op = (char)Read();
                                    GraphicState.Format.YDec = op - '0';
                                    break;
                            }
                        }
                        break;
                    }//case "FS"
                case "MO":
                    {
                        var op = ((char)Read()).ToString() + ((char)Read()).ToString();
                        if (EndOfStream)
                            return;
                        if (op == "IN")
                            GraphicState.Unit = Modes.Inches;
                        else
                            GraphicState.Unit = Modes.Millimeters;
                        break;
                    }
                case "AD"://aperture definition
                    var aperture = ParseApertureDefinition();
                    //todo register aperture to dictionary
                    /*
                     * a->unit = state->state->unit;
	    image->aperture[ano] = a;
	    dprintf("     In parse_rs274x, adding new aperture to aperture list ...\n");
	    gerbv_stats_add_aperture(stats->aperture_list,
				    -1, ano, 
				    a->type,
				    a->parameter);
	    gerbv_stats_add_to_D_list(stats->D_code_list,
				     ano);*/
                    break;

                case "AM":// aperture macro
                    {
                        /*
                        tmp_amacro = image->amacro;
                        image->amacro = parse_aperture_macro(fd);
                        if (image->amacro)
                        {
                            image->amacro->next = tmp_amacro;
# ifdef AMACRO_DEBUG
                            print_program(image->amacro);
#endif
                        }
                        else
                        {
                            string = g_strdup_printf(_("In file %s,\nfailed to parse aperture macro\n"),
                                         fd->filename);
                            gerbv_stats_add_error(stats->error_list,
                                     -1,
                                      string,
                                     GERBV_MESSAGE_ERROR);
                            g_free(string);
                        }
                        // return, since we want to skip the later back-up loop
                        return;
                        */
                        break;
                    }

                case "LP": // Level polarity
                    {
                        var op = (char)Read();
                        if (op == 'C')
                            GraphicState.LevelPolarity = Polarity.Clear;
                        else
                            GraphicState.LevelPolarity = Polarity.Dark;
                        break;
                    }

                case "SR": // Step Repeat
                    {
                        #region SR
                        /*
                                        // start by generating a new layer (duplicating previous layer settings 
                                        state->layer = gerbv_image_return_new_layer(state->layer);
                                        op[0] = gerb_fgetc(fd);
                                        if (op[0] == '*')
                                        { // Disable previous SR parameters 
                                            state->layer->stepAndRepeat.X = 1;
                                            state->layer->stepAndRepeat.Y = 1;
                                            state->layer->stepAndRepeat.dist_X = 0.0;
                                            state->layer->stepAndRepeat.dist_Y = 0.0;
                                            break;
                                        }
                                        while ((op[0] != '*') && (op[0] != EOF))
                                        {
                                            switch (op[0])
                                            {
                                                case 'X':
                                                    state->layer->stepAndRepeat.X = gerb_fgetint(fd, NULL);
                                                    break;
                                                case 'Y':
                                                    state->layer->stepAndRepeat.Y = gerb_fgetint(fd, NULL);
                                                    break;
                                                case 'I':
                                                    state->layer->stepAndRepeat.dist_X = gerb_fgetdouble(fd) / scale;
                                                    break;
                                                case 'J':
                                                    state->layer->stepAndRepeat.dist_Y = gerb_fgetdouble(fd) / scale;
                                                    break;
                                                default:
                                                    string = g_strdup_printf(_("In file %s,\nstep-and-repeat parameter error\n"),
                                                                 fd->filename);
                                                    gerbv_stats_add_error(stats->error_list,
                                                                 -1,
                                                                  string,
                                                                  GERBV_MESSAGE_ERROR);
                                                    g_free(string);
                                            }


                                             //* Repeating 0 times in any direction would disable the whole plot, and
                                             //* is probably not intended. At least one other tool (viewmate) seems
                                             //* to interpret 0-time repeating as repeating just once too.

                                            if (state->layer->stepAndRepeat.X == 0)
                                                state->layer->stepAndRepeat.X = 1;
                                            if (state->layer->stepAndRepeat.Y == 0)
                                                state->layer->stepAndRepeat.Y = 1;

                                            op[0] = gerb_fgetc(fd);
                                        }
                                        */
                        #endregion
                        break;
                    }//SR


            }

            //backspace one char in case we already hit the *
            //make sure we read until the trailing * char
            if (BaseStream != null)
                BaseStream.Position--;

            var c = Read();
            while (!EndOfStream && (char)c != '*')
                c = Read();

        }

        string GetString(char stopChar)
        {
            var retString = string.Empty;
            char c;
            while ((c = (char)Read()) != stopChar)
            {
                retString += c;
            }

            return retString;
        }
    }
}
