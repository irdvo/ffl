\ ==============================================================================
\
\     gsv23_expl - the gtk-server scrolled window example in the ffl
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

  tos-create fmt       \ Formatter


  : event>widget   ( c-addr u -- n = Convert the event string to a widget id )
    0. 2swap >number 2drop d>s
  ;


  0 value window                  \ Widgets/events
  
  
  : example
    gtk_init                      \ Initialise toolkit
    
    GTK_WINDOW_TOPLEVEL gtk_window_new to window
    
    s" GtkScrolledWindow example" window gtk_window_set_title
    0 window gtk_container_set_border_width
    300 300 window gtk_widget_set_size_request

    \ create a new scrolled window.
    gtk_scrolled_window_new >r
    
    10 r@ gtk_container_set_border_width
    
    \ the policy is one of GTK_POLICY AUTOMATIC, or GTK_POLICY_ALWAYS.
    \ GTK_POLICY_AUTOMATIC will automatically decide whether you need
    \ scrollbars, whereas GTK_POLICY_ALWAYS will always leave the scrollbars
    \ there.  The first one is the horizontal scrollbar, the second, 
    \ the vertical.
    GTK_POLICY_ALWAYS GTK_POLICY_AUTOMATIC r@ gtk_scrolled_window_set_policy

    r@ window gtk_container_add
    
    \ create a table of 10 by 10 squares.
    false 10 10 gtk_table_new >r
    
    \ set the spacing to 10 on x and 10 on y
    10 r@ gtk_table_set_row_spacings
    10 r@ gtk_table_set_col_spacings
    
    \ pack the table into the scrolled window 
    r@ r'@ gtk_scrolled_window_add_with_viewport
    
    \ this simply creates a grid of toggle buttons on the table
    \ to demonstrate the scrolled window.
    r>
    10 0 DO
      10 0 DO
        fmt tos-rewrite
        s" button (" fmt tos-write-string
        J            fmt tos-write-number
        [char] ,     fmt tos-write-char
        I            fmt tos-write-number
        [char] )     fmt tos-write-char
        fmt str-get gtk_toggle_button_new_with_label 
        
        2dup swap I 1+ I 2swap J 1+ J 2swap gtk_table_attach_defaults
        
        gtk_widget_show
      LOOP
    LOOP

       gtk_widget_show            \ table
    r> gtk_widget_show            \ scrolled
    
    window gtk_widget_show

                                  \ main loop
    BEGIN
      s" WAIT" gtk_server_callback
      event>widget window =       \ Only one event
    UNTIL
    
    0 gtk_exit
  ;

  example
  
  gsv+close drop
    
[ELSE]
  .( No gtk-server fifo, is the gtk-server running ? ) cr
[THEN]


\ ==============================================================================
