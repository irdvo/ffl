\ ==============================================================================
\
\         gsv_expl - the gtk-server interface example in the ffl
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
  
  
  : gsv-example
    gtk_init                    \ Initialise toolkit
                                \ Create toplevel window 
    GTK_WINDOW_TOPLEVEL gtk_window_new to window
    
    s" This is a title" window gtk_window_set_title
    
                                \ Layout the widgets with a table
    1 30 30 gtk_table_new >r
    
    r@ window gtk_container_add
    
                                \ Add a label to the window
    s" Hello world" gtk_label_new >r
    
    7 3 29 1 r@ r'@ gtk_table_attach_defaults
    
    r> gtk_widget_show
    
                                \ Add the exit button to the window
    s" Exit" gtk_button_new_with_label to exit-button   
    
    27 23 28 20 exit-button r@ gtk_table_attach_defaults

    exit-button gtk_widget_show
    
    r>          gtk_widget_show   \ Table
    
    window      gtk_widget_show   \ Window
    
                                  \ Main loop
    BEGIN
      s" WAIT" gtk_server_callback
      event>widget CASE
        window      OF true ENDOF
        exit-button OF true ENDOF
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
