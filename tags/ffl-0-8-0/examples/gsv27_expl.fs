\ ==============================================================================
\
\       gsv27_expl - the gtk-server menu example in the ffl
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
include ffl/act.fs
include ffl/spf.fs

\ Open the connection to the gtk-server and load all definitions from the gtk-server.cfg file

s" gtk-server.cfg" s" ffl-fifo" gsv+open 0= [IF]

  act-create events               \ Tree for widget -> xt
  act-create msgs                 \ Tree for widget -> message

  0 value menu                    \ Widgets/events


  : event>widget   ( c-addr u -- n = Convert the event string to a widget id )
    0. 2swap >number 2drop d>s
  ;


  \ Callbacks
  : deleted-event  ( widget -- flag = process the exit event )
    drop
    true                          \ Delete event -> stop
  ;


  : button_press   ( widget -- flag = Respond to a button-press by posting a menu pass in as widget )
    drop
    0 1 menu gtk_menu_popup     \ current time and left button
    false
  ;


  : menuitem_response ( widget -- flag = Process the menu item selection )
    msgs act-get IF
      str-get type cr
    THEN
    false
  ;


  : example
    gtk_init                    \ Initialise toolkit
    
    GTK_WINDOW_TOPLEVEL gtk_window_new >r
    
    ['] deleted-event r@ events act-insert
  
    100 200 r@ gtk_widget_set_size_request
    
    s" GTK Menu Test" r@ gtk_window_set_title
    
    \ Init the menu-widget, and remember -- never
    \ gtk_show_widget() the menu widget!! 
    \ This is the menu that holds the menu items, the one that
    \ will pop up when you click on the "Root Menu" in the app
    gtk_menu_new to menu

    \ Next we make a little loop that makes three menu-entries for "test-menu".
    \ Notice the call to gtk_menu_shell_append.  Here we are adding a list of
    \ menu items to our menu.  Normally, we'd also catch the "clicked"
    \ signal on each of the menu items and setup a callback for it,
    \ but it's omitted here to save space.

    3 0 DO
      I  
      \ Copy the names to the buf.
      str-new >r
      r@ spf" Test-undermenu - %d"
      
                            
      \ Create a new menu-item with a name... 
      r@ str-get gtk_menu_item_new_with_label >r

      \ ...and add it to the menu.
      r@ menu gtk_menu_shell_append

	    \ Do something interesting when the menuitem is selected 
      ['] menuitem_response r@ events act-insert
      r'@                   r@ msgs   act-insert

      \ Show the widget
      r> gtk_widget_show
      rdrop
    LOOP

    \ This is the root menu, and will be the label
    \ displayed on the menu bar.  There won't be a signal handler attached,
    \ as it only pops up the rest of the menu when pressed.
    s" Root Menu" gtk_menu_item_new_with_label

    dup gtk_widget_show           \ S: root-menu

    \ Now we specify that we want our newly created "menu" to be the menu
    \ for the "root menu" 
    menu over gtk_menu_item_set_submenu
    
    \ A vbox to put a menu and a button in:
    0 false gtk_vbox_new >r
    r@ r'@ gtk_container_add

    \ Create a menu-bar to hold the menus and add it to our main window 
    gtk_menu_bar_new >r
    2 false false r@ r'@ gtk_box_pack_start
    r@ gtk_widget_show

    \ And finally we append the menu-item to the menu-bar -- this is the
    \ "root" menu-item
    r> gtk_menu_shell_append      \ consumes root-menu

    \ Create a button to which to attach menu as a popup
    s" press me" gtk_button_new_with_label >r
    ['] button_press  r@ events act-insert

    2 true true r@ r'@ gtk_box_pack_end
    r> gtk_widget_show 
    
    r> gtk_widget_show            \ vbox

    \ always display the window as the last step so it all splashes on the screen at once.
    r> gtk_widget_show

                         
    BEGIN                         \ main loop, also duplicate event for callback word
      s" WAIT" gtk_server_callback
      event>widget
      dup events act-get IF
        execute
      ELSE
        drop false
      THEN
    UNTIL
    
    0 gtk_exit
  ;

  example
  
  gsv+close drop
    
[ELSE]
  .( No gtk-server fifo, is the gtk-server running ? ) cr
[THEN]


\ ==============================================================================
