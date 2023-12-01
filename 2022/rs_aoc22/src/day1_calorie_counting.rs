use std::fs::File;
use std::io::BufRead;
use std::io::BufReader;

#[allow(dead_code)]
#[derive(Clone)]
struct ElfInfo {
    elf_number: i32,
    calories: i32
}

#[allow(dead_code)]
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

#[allow(dead_code)]
pub fn part2(input_file: &String) -> Result<(), Box<dyn std::error::Error>> {
    // keep the state machine to enumerate the file, but collect results into a list instead of just keeping maxElf
    let mut current_elf_calories = 0;
    let mut last_line_was_blank = false;

    let mut elf_num = 1;
    let mut all_elves:Vec<ElfInfo> = Vec::with_capacity(300);
    
    let mut record_current = |current_elf_calories: &mut i32| {
        // println!("Elf {elf_num} is carrying {current_elf_calories} calories");
        all_elves.push(ElfInfo { elf_number: elf_num, calories: *current_elf_calories });
        
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

    all_elves.sort_by(|a,b| b.calories.cmp(&a.calories));
    let top_3:i32 = all_elves[0..3].iter().map(|elf| elf.calories).sum();

    println!("The top 3 elves together carry {} total calories", top_3);

    Ok(())
}