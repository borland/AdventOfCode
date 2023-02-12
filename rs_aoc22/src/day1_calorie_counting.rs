use std::fs::File;
use std::io::BufRead;
use std::io::BufReader;

struct ElfInfo {
    elf_number: i32,
    calories: i32
}

pub fn part1(input_file: &String) -> Result<(), Box<dyn std::error::Error>> {
    let mut current_elf_calories = 0;
    let mut last_line_was_blank = false;

    let mut elf_num = 1;
    let mut max_elf = ElfInfo { elf_number: 0, calories: 0 };
    
    let mut record_current = |current_elf_calories: &mut i32| {
        println!("Elf {elf_num} is carrying {current_elf_calories} calories");
        if *current_elf_calories > max_elf.calories {
            max_elf = ElfInfo { elf_number: elf_num, calories: *current_elf_calories };
        }
        elf_num += 1;
        *current_elf_calories = 0;
    };

    let file = File::open(input_file).expect("can't open file");
    let reader = BufReader::new(file);
    for l in reader.lines() {
        let line = l.expect("can't read line");

        if line == "" && !last_line_was_blank {
            last_line_was_blank = true;
            record_current(&mut current_elf_calories);
            continue;
        }

        let calories: i32 = line.trim().parse()?;
        last_line_was_blank = false;
        current_elf_calories += calories;
    }

    record_current(&mut current_elf_calories);
    println!("The elf carrying the most calories is {} with {} calories", max_elf.elf_number, max_elf.calories);

    Ok(())
}