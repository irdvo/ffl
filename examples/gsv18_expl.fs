\ ==============================================================================
\
\       gsv18_expl - the gtk-server event box example in the ffl
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


  0 value window                  \ Widgets/events
  0 value event-box
  0 value exit-button
  

  : example
    gtk_init                    \ Initialise toolkit
    
    GTK_WINDOW_TOPLEVEL gtk_window_new to window
    
    s" Event Box" window gtk_window_set_title
    
    10 window gtk_container_set_border_width
    
    \ Create an EventBox and add it to our toplevel window
    
    gtk_event_box_new to event-box
    event-box window gtk_container_add
    event-box gtk_widget_show
    
    \ Create a long label
    
    s" Click here to quit, quit, quit, quit, quit" gtk_label_new >r
    r@ event-box gtk_container_add
    r@ gtk_widget_show
    
    \ Clip it short
    
    20 110 r> gtk_widget_set_size_request
    
    \ And bind an action to it
    
    GDK_BUTTON_PRESS_MASK event-box gtk_widget_set_events 
    
    16 g_malloc to exit-button    \ Reserve event handle
    
    exit-button s" button_press_event" event-box gsv+server-connect 2drop
    
    \ Yet one more thing you need an X window for ... 
    
    event-box gtk_widget_realize
    58 gdk_cursor_new event-box gtk_widget_get_parent_window gdk_window_set_cursor  \ GDK_HAND1
    
    window gtk_widget_show
                                  \ main loop
    BEGIN
      s" WAIT" gtk_server_callback
      event>widget CASE
        window        OF true ENDOF
        exit-button   OF true ENDOF
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
