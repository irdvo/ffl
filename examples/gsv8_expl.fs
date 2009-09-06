\ ==============================================================================
\
\       gsv8_expl - the gtk-server label example in the ffl
\
\               Copyright (C) 2009  Dick van Oudheusden
\  
\ This library is free software; you can redistribute it and/or
\ modify it under the terms of the GNU General Public
\ License as published by the Free Software Foundation; either
\ version 2 of the License, or (at your option) any later version.
\
\ This library is distributed in the hope that it will be useful,
\ but WITHOUT ANY WARRANTY; without even the implied warranty of
\ MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
\ General Public License for more details.
\
\ You should have received a copy of the GNU General Public
\ License along with this library; if not, write to the Free
\ Software Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
\
\ ==============================================================================
\ 
\  $Date: 2009-05-28 17:35:58 $ $Revision: 1.11 $
\
\ ==============================================================================

include ffl/gsv.fs

\ Open the connection to the gtk-server and load all definitions from the gtk-server.cfg file

s" gtk-server.cfg" s" ffl-fifo" gsv+open 0= [IF]


  : event>widget   ( c-addr u -- n = Convert the event string to a widget id )
    0. 2swap >number 2drop d>s
  ;


  0 value window                  \ the widget events


  : example
    gtk_init                    \ Initialise toolkit
    
    GTK_WINDOW_TOPLEVEL gtk_window_new to window
    

    s" Label" window gtk_window_set_title

    5 window gtk_container_set_border_width

    5 false gtk_hbox_new >r

    r@ window gtk_container_add
    
    
    5 false gtk_vbox_new >r
    
    0 false false r@ r'@ gtk_box_pack_start
    
    
    s" Normal Label" gtk_frame_new >r
    
    s" This is a Normal label" gtk_label_new >r
    
    r@ r'@ gtk_container_add
    
    r> gtk_widget_show
    
    0 false false r@ r'@ gtk_box_pack_start
    
    r> gtk_widget_show
    
    
    
    s" Multi-line Label" gtk_frame_new >r
    
    s\" This is a Multi-line label.\nSecond line\nThird line" gtk_label_new >r
    
    r@ r'@ gtk_container_add
    
    r> gtk_widget_show
    
    0 false false r@ r'@ gtk_box_pack_start
    
    r> gtk_widget_show
  
  
    s" Left Justified Label" gtk_frame_new >r
    
    s\" This is a Left-Justified\nMulti-line label.\nThird      line" gtk_label_new >r
    
    GTK_JUSTIFY_LEFT r@ gtk_label_set_justify
    
    r@  r'@ gtk_container_add
    
    r> gtk_widget_show
    
    0 false false r@ r'@ gtk_box_pack_start
  
    r> gtk_widget_show
    
    
    s" Right Justified Label" gtk_frame_new >r
    
    s\" This is a Right-Justified\nMulti-line label.\nFourth line, (j/k)" gtk_label_new >r
    
    GTK_JUSTIFY_RIGHT r@ gtk_label_set_justify
    
    r@ r'@ gtk_container_add

    r> gtk_widget_show
    
    0 false false r@ r'@ gtk_box_pack_start
    
    r> gtk_widget_show
    
    
    r> gtk_widget_show            \ vbox


    5 false gtk_vbox_new >r
    
    0 false false r@ r'@ gtk_box_pack_start
    

    s" Line wrapped label" gtk_frame_new >r
    
    s\" This is an example of a line-wrapped label.  It should not be taking up the entire             width allocated to it, but automatically wraps the words to fit.  The time has come, for all good men, to come to the aid of their party.  The sixth sheik's six sheep's sick.\n     It supports multiple paragraphs correctly, and  correctly   adds many          extra  spaces. " gtk_label_new >r
    
    true r@ gtk_label_set_line_wrap
    
    r@ r'@ gtk_container_add
    
    r> gtk_widget_show
    
    0 false false r@ r'@ gtk_box_pack_start
  
    r> gtk_widget_show
    
    
    s" Filled, wrapped label" gtk_frame_new >r
    
    s\" This is an example of a line-wrapped, filled label.  It should be taking up the entire              width allocated to it.  Here is a sentence to prove my point.  Here is another sentence. Here comes the sun, do de do de do.\n    This is a new paragraph.\n    This is another newer, longer, better paragraph.  It is coming to an end, unfortunately." gtk_label_new >r

    GTK_JUSTIFY_FILL r@ gtk_label_set_justify
    
    true r@ gtk_label_set_line_wrap
    
    r@ r'@ gtk_container_add
    
    r> gtk_widget_show
    
    0 false false r@ r'@ gtk_box_pack_start
    
    r> gtk_widget_show
  
  
    s" Underlined label" gtk_frame_new >r
    
    s\" This label is underlined!\nThis one is underlined in quite a funky fashion" gtk_label_new >r
    
    GTK_JUSTIFY_LEFT r@ gtk_label_set_justify

    s" _________________________ _ _________ _ ______     __ _______ ___" r@ gtk_label_set_pattern
			 
    r@ r'@ gtk_container_add
    
    r> gtk_widget_show
    
    0 false false r@ r'@ gtk_box_pack_start
    
    r> gtk_widget_show
    
    r>     gtk_widget_show            \ vbox
    r>     gtk_widget_show            \ hbox
    window gtk_widget_show
   
                                  \ main loop
    BEGIN
      s" WAIT" gtk_server_callback
      event>widget CASE
        window       OF true ENDOF
        false swap
      ENDCASE
    UNTIL
    
    0 gtk_exit
  ;

  example
  
  gsv+close drop
    
[ELSE]
  .( No gtk-server fifo, is the gtk-server running ? ) cr
[THEN]


\ ==============================================================================
