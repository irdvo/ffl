\ ==============================================================================
\
\         gsv2_expl - the gtk-server button example in the ffl
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


  0 value window                \ the event widgets
  0 value exit-button
  0 value button1
  0 value button2


  : gsv-example
    gtk_init                      \ Initialise toolkit
                                  \ Create toplevel window
    GTK_WINDOW_TOPLEVEL gtk_window_new to window

    s" Hello Buttons!" window gtk_window_set_title
    
    10 window gtk_container_set_border_width
    
                                  \ Layout the widgets with a hbox
    0 false gtk_hbox_new >r
    
    r@ window gtk_container_add

                                  \ Add a button to the window with the click event
    s" Button 1" gtk_button_new_with_label to button1

    0 true true button1 r@ gtk_box_pack_start
    
    button1 gtk_widget_show
        
                                  \ Add a button to the window with the click event
    s" Button 2" gtk_button_new_with_label to button2
    
    0 true true button2 r@ gtk_box_pack_start
    
    button2 gtk_widget_show

                                  \ Add a button to the window with the click event
    s" Exit" gtk_button_new_with_label to exit-button
    
    0 true true exit-button r@ gtk_box_pack_start
    
    exit-button gtk_widget_show

    r>          gtk_widget_show   \ Box
    
    window      gtk_widget_show   \ Window

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

  gsv-example
  
  gsv+close drop
    
[ELSE]
  .( No gtk-server fifo, is the gtk-server running ? ) cr
[THEN]


\ ==============================================================================
