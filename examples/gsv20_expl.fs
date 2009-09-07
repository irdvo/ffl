\ ==============================================================================
\
\       gsv20_expl - the gtk-server frame example in the ffl
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


  0 value window                \ Widgets/events
  
  
  : example
    gtk_init                    \ Initialise toolkit
    
    GTK_WINDOW_TOPLEVEL gtk_window_new to window
    
     s" Frame Example" window gtk_window_set_title

    300 300 window gtk_widget_set_size_request
    
    10 window gtk_container_set_border_width

    \ Create a Frame
    s" " gtk_frame_new >r
    r@ window gtk_container_add

    \ Set the frame's label
    s" GTK Frame Widget" r@ gtk_frame_set_label

    \ Align the label at the right of the frame
    0.0E+0 1.0E+0 r@ gtk_frame_set_label_align

    \ Set the style of the frame 
    GTK_SHADOW_ETCHED_OUT r@ gtk_frame_set_shadow_type

    r> gtk_widget_show
  
    \ Display the window
    window gtk_widget_show
 
                                  \ main loop
    BEGIN
      s" WAIT" gtk_server_callback
      event>widget window =       \ Done if window event
    UNTIL
    
    0 gtk_exit
  ;

  example
  
  gsv+close drop
    
[ELSE]
  .( No gtk-server fifo, is the gtk-server running ? ) cr
[THEN]


\ ==============================================================================
