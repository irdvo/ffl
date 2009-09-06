\ ==============================================================================
\
\         gsv4_expl - the gtk-server table example in the ffl
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

\ Load all definitions from the gtk-server.cfg file

s" gtk-server.cfg" s" ffl-fifo" gsv+open 0= [IF]

  : event>widget   ( c-addr u -- n = Convert the event string to a widget id )
    0. 2swap >number 2drop d>s
  ;


  0 value window                \ the event widgets
  0 value exit-button
  0 value button1
  0 value button2

  
  : example
    gtk_init                    \ Initialise toolkit
                                \ Create toplevel window and connect delete-event with word
    GTK_WINDOW_TOPLEVEL gtk_window_new to window

    s" Table" window gtk_window_set_title

    20 window gtk_container_set_border_width

    true 2 2 gtk_table_new >r         \  Create a 2x2 table 

    r@ window gtk_container_add       \ Put the table in the main window

    s" Button 1" gtk_button_new_with_label to button1
     
                               \ Insert button 1 into the upper left quadrant of the table
    1 0 1 0 button1 r@ gtk_table_attach_defaults

    button1 gtk_widget_show

    s" button 2" gtk_button_new_with_label to button2
    
                              \ Insert button 2 into the upper right quadrant of the table 
    1 0 2 1 button2 r@ gtk_table_attach_defaults
    
    button2 gtk_widget_show

                              \ Create "Quit" button
    s" Quit" gtk_button_new_with_label to exit-button
    
                      \ Insert the quit button into the both lower quadrants of the table
    2 1 2 0 exit-button r@ gtk_table_attach_defaults
    
    exit-button gtk_widget_show

    r>          gtk_widget_show   \ Table
    
    window      gtk_widget_show   

                                \ Main loop
    BEGIN
      s" WAIT" gtk_server_callback
      event>widget CASE
        window      OF true ENDOF
        exit-button OF true ENDOF
        button1     OF ." Hello again - button1 was pressed" cr false ENDOF
        button2     OF ." Hello again - button2 was pressed" cr false ENDOF
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
